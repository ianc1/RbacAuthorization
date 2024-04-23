namespace RbacAuthorization.Tests;
using System;

using FluentAssertions;

using static RbacAuthorization.Tests.TestHarness.TestValues;

public class AuthorizePermissionAttributeTests
{
    [Fact]
    public void Constructor_should_throw_when_permission_parameter_is_null()
    {
        // arrange / act
        var act = () => new AuthorizePermissionAttribute(permission: null!);

        // assert
        act.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("permission");
    }

    [Fact]
    public void Permission_property_should_contain_the_name_of_the_permission()
    {
        // arrange
        var attribute = new AuthorizePermissionAttribute(ReadPermission);

        // act
        var permission = attribute.Permission;

        // assert
        permission.Should().Be(ReadPermission);
    }

    [Fact]
    public void Policy_property_should_contain_the_name_of_the_permission_prefixed_with_the_policy_name()
    {
        // arrange
        var attribute = new AuthorizePermissionAttribute(ReadPermission);

        // act
        var policy = attribute.Policy;

        // assert
        policy.Should().Be($"PermissionPolicy:{ReadPermission}");
    }
}
