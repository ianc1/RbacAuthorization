namespace RbacAuthorization.Tests;

using FluentAssertions;

using static RbacAuthorization.Tests.TestHarness.TestValues;

public class RoleTests
{
    [Fact]
    public void Constructor_should_throw_when_name_parameter_is_null()
    {
        // arrange / act
        var act = () => new Role(name: null!, pathScope: UserScope);

        // assert
        act.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("name");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("name:with:colons")]
    public void Constructor_should_throw_when_name_parameter_is_invalid(string? invalidValue)
    {
        // arrange / act
        var act = () => new Role(name: invalidValue!, pathScope: UserScope);

        // assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Name must not be empty or contain colons.");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(" /invalid/leading/whitespace")]
    [InlineData("/invalid/trailing/whitespace ")]
    [InlineData("unsupported/relative/path")]
    [InlineData("/invalid/trailing/slash/")]
    [InlineData("http://unsupported/full/url")]
    [InlineData("/unsupported/{parameter}")]
    public void Constructor_should_throw_when_pathScope_parameter_is_invalid(string invalidValue)
    {
        // arrange / act
        var act = () => new Role(name: UserRoleName, pathScope: invalidValue);

        // assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("PathScope must be a valid absolute URL path and not end with a slash.");
    }

    [Fact]
    public void Parse_should_throw_when_role_parameter_is_null()
    {
        // arrange / act
        var act = () => Role.Parse(role: null!);

        // assert
        act.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("role");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(":/path/scope")]
    [InlineData(" :/path/scope")]
    public void Parse_should_throw_when_name_portion_of_the_role_is_invalid(string invalidValue)
    {
        // arrange / act
        var act = () => Role.Parse(invalidValue);

        // assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Name must not be empty or contain colons.");
    }

    [Theory]
    [InlineData("name:")]
    [InlineData("name: ")]
    [InlineData("name: /invalid/leading/whitespace")]
    [InlineData("name:/invalid/trailing/whitespace ")]
    [InlineData("name:unsupported/relative/path")]
    [InlineData("name:/invalid/trailing/slash/")]
    [InlineData("name:http://unsupported/full/url")]
    [InlineData("name:/unsupported/{parameter}")]
    public void Parse_should_throw_when_pathScope_portion_of_the_role_is_invalid(string invalidValue)
    {
        // arrange / act
        var act = () => Role.Parse(invalidValue);

        // assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("PathScope must be a valid absolute URL path and not end with a slash.");
    }

    [Theory]
    [InlineData("MyName", "/my/scope", "MyName", "/my/scope", true)]
    [InlineData("MyName", "/my/scope", "DifferentName", "/my/scope", false)]
    [InlineData("MyName", "/my/scope", "MyName", "/different/scope", false)]
    public void Equals_should_return_true_when_two_Roles_have_all_the_same_values(
    string name1, string scope1,
    string name2, string scope2,
    bool expectedOutcome)
    {
        // arrange
        var role1 = new Role(name1, scope1);
        var role2 = new Role(name2, scope2);

        // act
        var isEqual = role1.Equals(role2);

        var isEqualOperator = role1 == role2;

        var isNotEqualOperator = role1 != role2;

        var isGetHashCodeEqual = role1.GetHashCode() == role2.GetHashCode();

        var isCompareToEqual = role1.CompareTo(role2) == 0;

        // assert
        isEqual.Should().Be(expectedOutcome);

        isEqualOperator.Should().Be(expectedOutcome);

        isNotEqualOperator.Should().Be(!expectedOutcome);

        isGetHashCodeEqual.Should().Be(expectedOutcome);

        isCompareToEqual.Should().Be(expectedOutcome);
    }
}
