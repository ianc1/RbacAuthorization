namespace RbacAuthorization.Tests;

using FluentAssertions;

using RbacAuthorization;

using static RbacAuthorization.Tests.TestHarness.TestValues;

public class RoleDefinitionTests
{
    [Fact]
    public void Constructor_should_throw_when_name_parameter_is_null()
    {
        // arrange / act
        var act = () => new RoleDefinition(name: null!, [ReadPermission]);

        // assert
        act.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("name");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("name:with:colons")]
    public void Constructor_should_throw_when_name_parameter_is_invalid(string invalidValue)
    {
        // arrange / act
        var act = () => new RoleDefinition(name: invalidValue, [ReadPermission]);

        // assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Name must not be empty or contain colons.");
    }

    [Theory]
    [InlineData(ReadPermission, null)]
    [InlineData(ReadPermission, "")]
    [InlineData(ReadPermission, " ")]
    public void Constructor_should_throw_when_permissions_parameter_is_invalid(params string?[] invalidValue)
    {
        // arrange / act
        var act = () => new RoleDefinition(UserRoleName, permissions: invalidValue!);

        // assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Permissions must contain at least one permission and each permission must not be empty.");
    }

    [Fact]
    public void Constructor_should_throw_when_permissions_parameter_is_null()
    {
        // arrange / act
        var act = () => new RoleDefinition(UserRoleName, permissions: null!);

        // assert
        act.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("permissions");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(" /invalid/leading/whitespace")]
    [InlineData("/invalid/trailing/whitespace ")]
    [InlineData("unsupported/relative/path")]
    [InlineData("/invalid/trailing/slash/")]
    [InlineData("http://unsupported/full/url")]
    [InlineData("/unsupported/partial/leading{parameter}")]
    [InlineData("/unsupported/partial/{parameter}trailing")]
    public void Constructor_should_throw_when_pathScope_parameter_is_invalid(string invalidValue)
    {
        // arrange / act
        var act = () => new RoleDefinition(UserRoleName, permissions: [ReadPermission], pathScope: invalidValue);

        // assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("PathScope must be a valid absolute URL path with optional parameter segments and not end with a slash.");
    }

    [Theory]
    [InlineData("Basic", null, "Basic")]
    [InlineData("FixedScope", "/users/me", "FixedScope:/users/me")]
    [InlineData("VariableScope", "/organizations/{OrganizationId}/projects/{ProjectId}", "VariableScope:/organizations/{OrganizationId}/projects/{ProjectId}")]

    public void RoleDefinition_should_have_the_expected_properties(string name, string? scope, string expectedValue)
    {
        // arrange / act
        var roleDefinition = new RoleDefinition(name, [ReadPermission], scope);

        // assert
        roleDefinition.Should().NotBeNull();

        roleDefinition.ToString().Should().Be(expectedValue);
    }

    [Theory]
    [InlineData("Basic", null, new string[0], "Basic")]
    [InlineData("FixedScope", "/users/me", new string[0], "FixedScope:/users/me")]
    [InlineData("VariableScope", "/organizations/{OrganizationId}/projects/{ProjectId}", new[] { "123", "456" }, "VariableScope:/organizations/123/projects/456")]
    public void CreateRole_should_return_a_role_generated_using_the_role_definition(string name, string? scope, string[] scopeParameters, string expectedRole)
    {
        // arrange
        var roleDefinition = new RoleDefinition(name, [ReadPermission], scope);

        // act
        var role = roleDefinition.CreateRole(scopeParameters);

        // assert
        role.Should().NotBeNull();

        role.ToString().Should().Be(expectedRole);
    }

    [Theory]
    [InlineData(null, 0, new[] { "123" })]
    [InlineData("/users/me", 0, new[] { "123" })]
    [InlineData("/users/{UserId}", 1, new string[0])]
    [InlineData("/organizations/{OrganizationId}/projects/{ProjectId}", 2, new[] { "123" })]
    [InlineData("/organizations/{OrganizationId}/projects/{ProjectId}", 2, new[] { "123", "456", "789" })]
    public void CreateRole_should_throw_when_supplied_scope_parameters_do_not_match_the_role_definition(string? scope, int expectedParameterCount, string[] actualParameters)
    {
        // arrange
        var roleDefinition = new RoleDefinition(UserRoleName, [ReadPermission], scope);

        // act
        var act = () => roleDefinition.CreateRole(actualParameters);

        // assert
        var expectedErrorMessage = scope is null
            ? "RoleDefinition does not contain a PathScope so PathScopeParameters should not be supplied."
            : $"PathScope definition requires {expectedParameterCount} parameters but {actualParameters.Length} were supplied.";

        act.Should().Throw<ArgumentException>()
            .WithMessage(expectedErrorMessage);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]

    public void CreateRole_should_throw_when_supplied_scope_parameters_are_empty(string? emptyParameter)
    {
        // arrange
        var roleDefinition = new RoleDefinition(UserRoleName, [ReadPermission], "/organizations/{OrganizationId}");

        // act
        var act = () => roleDefinition.CreateRole(emptyParameter!);

        // assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("PathScope definition parameters must not be empty.");
    }

    [Theory]
    [InlineData(null, "/", "MyRole")]
    [InlineData("/users/me", "/users/me", "MyRole:/users/me")]
    [InlineData("/users/me", "/users/me/profile", "MyRole:/users/me")]
    [InlineData("/organizations/{OrganizationId}/projects/{ProjectId}", "/organizations/123/projects/456", "MyRole:/organizations/123/projects/456")]
    [InlineData("/organizations/{OrganizationId}/projects/{ProjectId}", "/organizations/123/projects/456/tasks", "MyRole:/organizations/123/projects/456")]
    public void HasPermission_should_return_true_and_the_required_role_when_the_role_definition_has_the_required_permission_for_the_request_path(string? pathScope, string requestPath, string expectedRequiredRole)
    {
        // arrange
        var roleDefinition = new RoleDefinition("MyRole", [ReadPermission], pathScope);

        // / act
        var hasPermission = roleDefinition.HasPermission(ReadPermission, requestPath, out var requiredRole);

        // assert
        hasPermission.Should().BeTrue();

        requiredRole.Should().NotBeNull();

        requiredRole!.ToString().Should().Be(expectedRequiredRole);
    }

    [Theory]
    [InlineData("Unknown.Permission", "/users/me", "Role definition does not have the permission but is scoped for the path.")]
    [InlineData("Tasks.Read", "/incorrect/path", "Role definition has the permission but the role is NOT scoped for the path.")]
    public void HasPermission_should_return_false_and_a_null_required_role_when_the_role_definition_does_NOT_have_the_required_permission_for_the_request_path(string requiredPermission, string requestPath, string description)
    {
        // arrange
        var roleDefinition = new RoleDefinition(UserRoleName, [ReadPermission], UserScope);

        // act
        var hasPermission = roleDefinition.HasPermission(requiredPermission, requestPath, out var requiredRole);

        // assert
        hasPermission.Should().BeFalse(description);

        requiredRole.Should().BeNull(description);
    }

    [Theory]
    [InlineData("Basic", null, null, "Basic")]
    [InlineData("FixedScope", "/users/me", null, "FixedScope:/users/me")]
    [InlineData("VariableScope", "/organizations/{OrganizationId}/projects/{ProjectId}", new string[] { "123", "456" }, "VariableScope:/organizations/123/projects/456")]
    public void ToString_should_contain_the_name_and_scope(string name, string? scope, string[]? scopeParameters, string expectedRole)
    {
        // arrange
        var roleDefinition = new RoleDefinition(name, [ReadPermission], scope);

        // act
        var role = roleDefinition.CreateRole(scopeParameters ?? []);

        // assert
        role.Should().NotBeNull();

        role.ToString().Should().Be(expectedRole);
    }

    [Theory]
    [InlineData("MyName", "MyPermission", "/my/scope", "MyName", "MyPermission", "/my/scope", true)]
    [InlineData("MyName", "MyPermission", "/my/scope", "DifferentName", "MyPermission", "/my/scope", false)]
    [InlineData("MyName", "MyPermission", "/my/scope", "MyName", "DifferentPermission", "/my/scope", false)]
    [InlineData("MyName", "MyPermission", "/my/scope", "MyName", "MyPermission", "/different/scope", false)]
    public void Equals_should_return_true_when_two_RoleDefinitions_have_all_the_same_values(
        string name1, string permission1, string scope1,
        string name2, string permission2, string scope2,
        bool expectedOutcome)
    {
        // arrange
        var roleDefinition1 = new RoleDefinition(name1, [permission1], scope1);
        var roleDefinition2 = new RoleDefinition(name2, [permission2], scope2);

        // act
        var isEqual = roleDefinition1.Equals(roleDefinition2);

        var isEqualOperator = roleDefinition1 == roleDefinition2;

        var isNotEqualOperator = roleDefinition1 != roleDefinition2;

        var isGetHashCodeEqual = roleDefinition1.GetHashCode() == roleDefinition2.GetHashCode();

        var isCompareToEqual = roleDefinition1.CompareTo(roleDefinition2) == 0;

        // assert
        isEqual.Should().Be(expectedOutcome);

        isEqualOperator.Should().Be(expectedOutcome);

        isNotEqualOperator.Should().Be(!expectedOutcome);

        isGetHashCodeEqual.Should().Be(expectedOutcome);

        isCompareToEqual.Should().Be(expectedOutcome);
    }
}
