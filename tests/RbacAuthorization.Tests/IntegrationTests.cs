namespace RbacAuthorization.Tests;

using FluentAssertions;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using RbacAuthorization.ConfigureRoles;
using RbacAuthorization.DependencyInjection;
using RbacAuthorization.Locators;
using RbacAuthorization.Tests.TestHarness;

using static RbacAuthorization.Tests.TestHarness.TestValues;

public class IntegrationTests
{
    private readonly HttpContext httpContext = new DefaultHttpContext();


    [Fact]
    public async Task AuthorizeAsync_should_return_succeeded_false_when_user_is_not_authenticated()
    {
        // arrange
        var authorizationService = AuthorizationServiceBuilder.CreateWithRbacAuthorization(options =>
        {
            options.AddInMemoryRoleDefinitions(
                new RoleDefinition(UserRoleName, [ReadPermission]));
        });

        var user = TestUserBuilder.UnauthenticatedUser();

        // act
        var result = await authorizationService.AuthorizeAsync(user, httpContext, ReadPermissionPolicy);

        // assert
        result.Succeeded.Should().BeFalse();
    }

    [Fact]
    public async Task AuthorizeAsync_should_return_succeeded_false_when_user_does_not_have_an_identity()
    {
        // arrange
        var authorizationService = AuthorizationServiceBuilder.CreateWithRbacAuthorization(options =>
        {
            options.AddInMemoryRoleDefinitions(
                new RoleDefinition(UserRoleName, [ReadPermission]));
        });

        var user = TestUserBuilder.NoIdentity();

        // act
        var result = await authorizationService.AuthorizeAsync(user, httpContext, ReadPermissionPolicy);

        // assert
        result.Succeeded.Should().BeFalse();
    }

    [Fact]
    public async Task AuthorizeAsync_should_return_succeeded_false_when_resource_parameter_is_not_an_httpContent()
    {
        // arrange
        var wrongParameter = false;

        var authorizationService = AuthorizationServiceBuilder.CreateWithRbacAuthorization(options =>
        {
            options.AddInMemoryRoleDefinitions(
                new RoleDefinition(UserRoleName, [ReadPermission]));
        });

        var user = new TestUserBuilder()
            .SetUserId(TestUserId)
            .AddRole(UserRoleName)
            .Build();

        // act
        var result = await authorizationService.AuthorizeAsync(user, resource: wrongParameter, ReadPermissionPolicy);

        // assert
        result.Succeeded.Should().BeFalse();
    }

    [Fact]
    public async Task AuthorizeAsync_should_return_succeeded_false_when_userId_not_found()
    {
        // arrange
        var authorizationService = AuthorizationServiceBuilder.CreateWithRbacAuthorization(options =>
        {
            options.AddClaimsPrincipalUserId("wrong-userId-claim");

            options.AddInMemoryRoleDefinitions(
                new RoleDefinition(UserRoleName, [ReadPermission]));
        });

        var user = new TestUserBuilder()
            .SetUserId(TestUserId)
            .AddRole(UserRoleName)
            .Build();

        // act
        var result = await authorizationService.AuthorizeAsync(user, httpContext, ReadPermissionPolicy);

        // assert
        result.Succeeded.Should().BeFalse();
    }

    [Theory]
    [InlineData(ReadPermissionPolicy, true)]
    [InlineData(WritePermissionPolicy, false)]
    public async Task AuthorizeAsync_should_return_succeeded_true_when_user_has_a_role_containing_the_required_permission(string requiredPolicy, bool expectedResult)
    {
        // arrange
        var authorizationService = AuthorizationServiceBuilder.CreateWithRbacAuthorization(options =>
        {
            options.AddInMemoryRoleDefinitions(
                new RoleDefinition(UserRoleName, [ReadPermission]),
                new RoleDefinition(AdminRoleName, [WritePermission, ReadPermission]));
        });

        var user = new TestUserBuilder()
            .SetUserId(TestUserId)
            .AddRole(UserRoleName)
            .Build();

        // act
        var result = await authorizationService.AuthorizeAsync(user, httpContext, requiredPolicy);

        // assert
        Assert.Equal(expectedResult, result.Succeeded);
    }

    [Theory]
    [InlineData("/organizations/123/projects/456", true)]
    [InlineData("/organizations/123/projects/456/tasks", true)]
    [InlineData("/organizations/111/projects/456/tasks", false)]
    [InlineData("/organizations/123/projects/111/tasks", false)]
    [InlineData("/organizations/111/projects/111/tasks", false)]
    [InlineData("/other", false)]
    public async Task AuthorizeAsync_should_return_succeeded_true_when_user_has_a_scoped_role_for_the_requested_path_containing_the_required_permission(string requestPath, bool expectedResult)
    {
        // arrange
        httpContext.Request.Path = requestPath;

        var authorizationService = AuthorizationServiceBuilder.CreateWithRbacAuthorization(options =>
        {
            options.AddInMemoryRoleDefinitions(
                new RoleDefinition(ProjectAdminRoleName, [ReadPermission], ProjectAdminScopeDefinition),
                new RoleDefinition(AdminRoleName, [WritePermission, ReadPermission]));
        });

        var user = new TestUserBuilder()
            .SetUserId(TestUserId)
            .AddRole(ProjectAdminRole)
            .Build();

        // act
        var result = await authorizationService.AuthorizeAsync(user, httpContext, ReadPermissionPolicy);

        // assert
        Assert.Equal(expectedResult, result.Succeeded);
    }

    [Theory]
    [InlineData("sub")]
    [InlineData("email")]
    [InlineData("objectId")]
    [InlineData("samAccountName")]
    [InlineData("oid")]
    public async Task AuthorizeAsync_should_return_succeeded_true_when_configuring_a_different_ClaimType_containing_the_UserId(string claimTypeContainingUserId)
    {
        // arrange
        var authorizationService = AuthorizationServiceBuilder.CreateWithRbacAuthorization(options =>
        {
            options.AddClaimsPrincipalUserId(claimTypeContainingUserId);

            options.AddInMemoryRoleDefinitions(
                new RoleDefinition(UserRoleName, [ReadPermission]));
        });

        var user = new TestUserBuilder()
            .SetUserIdClaimName(claimTypeContainingUserId)
            .SetUserId(TestUserId)
            .AddRole(UserRoleName)
            .Build();

        // act
        var result = await authorizationService.AuthorizeAsync(user, httpContext, ReadPermissionPolicy);

        // assert
        Assert.True(result.Succeeded);
    }

    [Theory]
    [InlineData("group")]
    [InlineData("customClaim")]
    [InlineData("custom-claim")]
    public async Task AuthorizeAsync_should_return_succeeded_true_when_configuring_a_different_ClaimType_containing_the_user_roles(string claimTypeContainingUserRoles)
    {
        // arrange
        var authorizationService = AuthorizationServiceBuilder.CreateWithRbacAuthorization(options =>
        {
            options.AddClaimsPrincipalUserRoles(claimTypeContainingUserRoles);

            options.AddInMemoryRoleDefinitions(
                new RoleDefinition(UserRoleName, [ReadPermission]));
        });

        var user = new TestUserBuilder()
            .SetUserId(TestUserId)
            .SetRoleClaimName(claimTypeContainingUserRoles)
            .AddRole(UserRoleName)
            .Build();

        // act
        var result = await authorizationService.AuthorizeAsync(user, httpContext, ReadPermissionPolicy);

        // assert
        Assert.True(result.Succeeded);
    }

    [Theory]
    [InlineData(UserRoleName, true)]
    [InlineData(AdminRoleName, false)]
    public async Task AuthorizeAsync_should_return_succeeded_true_when_using_a_custom_UserRolesLocator(string role, bool expectedResult)
    {
        // arrange
        var authorizationService = AuthorizationServiceBuilder.CreateWithRbacAuthorization(
            options =>
            {
                options.AddInMemoryRoleDefinitions(
                    new RoleDefinition(UserRoleName, [ReadPermission]));

                options.Services.AddSingleton<IUserRolesLocator>(new TestUserRolesLocator(Role.Parse(role)));
            });

        var user = new TestUserBuilder()
            .SetUserId(TestUserId)
            .Build();

        // act
        var result = await authorizationService.AuthorizeAsync(user, httpContext, ReadPermissionPolicy);

        // assert
        Assert.Equal(expectedResult, result.Succeeded);
    }

    [Theory]
    [InlineData(ReadPermission, true)]
    [InlineData(WritePermission, false)]
    public async Task AuthorizeAsync_should_return_succeeded_true_when_using_a_custom_RoleDefinitionsLocator(string rolePermission, bool expectedResult)
    {
        // arrange
        var authorizationService = AuthorizationServiceBuilder.CreateWithRbacAuthorization(
            options =>
            {
                options.Services.AddSingleton<IRoleDefinitionsLocator>(new TestRoleDefinitionsLocator(UserRoleName, rolePermission));
            });

        var user = new TestUserBuilder()
            .SetUserId(TestUserId)
            .AddRole(UserRoleName)
            .Build();

        // act
        var result = await authorizationService.AuthorizeAsync(user, httpContext, ReadPermissionPolicy);

        // assert
        Assert.Equal(expectedResult, result.Succeeded);
    }
}