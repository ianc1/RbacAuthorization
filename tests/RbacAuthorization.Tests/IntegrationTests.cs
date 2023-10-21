namespace RbacAuthorization.Tests;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using RbacAuthorization.ConfigureRoles;
using RbacAuthorization.DependencyInjection;
using RbacAuthorization.Tests.TestHarness;

using static RbacAuthorization.Tests.TestHarness.TestValues;

public class IntegrationTests
{
    private readonly HttpContext httpContext = new DefaultHttpContext();

    [Theory]
    [InlineData(ReadPermission, true)]
    [InlineData(WritePermission, false)]
    public async Task AuthorizeAsync_should_return_succeeded_true_when_user_has_a_role_containing_the_required_permission(string requiredPermission, bool expectedResult)
    {
        // arrange
        var authorizationService = AuthorizationServiceBuilder.CreateWithRbacAuthorization(options =>
        {
            options.ConfigureRoles(roles => roles
                .Add(SupervisorRole).WithPermissions(ReadPermission)
                .Add(ManagerRole).WithPermissions(WritePermission, ReadPermission));
        });

        var user = new TestUserBuilder()
            .SetUserId(TestUserId)
            .AddRole(SupervisorRole)
            .Build();

        // act
        var result = await authorizationService.AuthorizeAsync(user, httpContext, requiredPermission);

        // assert
        Assert.Equal(expectedResult, result.Succeeded);
    }

    [Theory]
    [InlineData(ReadPermission, true)]
    [InlineData(WritePermission, false)]
    public async Task AuthorizeAsync_should_return_succeeded_true_when_user_has_a_tenant_role_containing_the_required_permission(string requiredPermission, bool expectedResult)
    {
        // arrange
        httpContext.Request.RouteValues = new RouteValueDictionary()
        {
            { DefaultValues.DefaultTenantIdVariableName, TestTenantId },
        };

        var authorizationService = AuthorizationServiceBuilder.CreateWithRbacAuthorization(options =>
        {
            options.ConfigureRoles(roles => roles
                .Add(TenantSupervisorRole).WithPermissions(ReadPermission)
                .Add(TenantManagerRole).WithPermissions(WritePermission, ReadPermission));
        });

        var user = new TestUserBuilder()
            .SetUserId(TestUserId)
            .AddRole(TestTenantSupervisorRole)
            .Build();

        // act
        var result = await authorizationService.AuthorizeAsync(user, httpContext, requiredPermission);

        // assert
        Assert.Equal(expectedResult, result.Succeeded);
    }

    [Theory]
    [InlineData("AccountId")]
    [InlineData("AccountName")]
    [InlineData("User-Id")]
    [InlineData("User_Id")]
    [InlineData("userId")]
    public async Task AuthorizeAsync_should_return_succeeded_true_when_using_a_custom_TenantIdVariableName(string tenantIdVariableName)
    {
        // arrange
        httpContext.Request.RouteValues = new RouteValueDictionary()
        {
            { tenantIdVariableName, TestTenantId },
        };

        var authorizationService = AuthorizationServiceBuilder.CreateWithRbacAuthorization(options =>
        {
            options.TenantIdVariableName = tenantIdVariableName;

            options.ConfigureRoles(roles => roles
                .Add($"TestApp.${tenantIdVariableName}.Supervisor").WithPermissions(ReadPermission)
                .Add($"TestApp.${tenantIdVariableName}.Manager").WithPermissions(WritePermission, ReadPermission));
        });

        var user = new TestUserBuilder()
            .SetUserId(TestUserId)
            .AddRole($"TestApp.{TestTenantId}.Supervisor")
            .Build();

        // act
        var result = await authorizationService.AuthorizeAsync(user, httpContext, ReadPermission);

        // assert
        Assert.True(result.Succeeded);
    }

    [Theory]
    [InlineData("email")]
    [InlineData("objectId")]
    [InlineData("samAccountName")]
    public async Task AuthorizeAsync_should_return_succeeded_true_when_using_a_custom_ClaimTypeContainingUserId(string claimTypeContainingUserId)
    {
        // arrange
        var authorizationService = AuthorizationServiceBuilder.CreateWithRbacAuthorization(options =>
        {
            options.UserIdClaimType = claimTypeContainingUserId;

            options.ConfigureRoles(roles => roles
                .Add(SupervisorRole).WithPermissions(ReadPermission));
        });

        var user = new TestUserBuilder()
            .SetUserIdClaimName(claimTypeContainingUserId)
            .SetUserId(TestUserId)
            .AddRole(SupervisorRole)
            .Build();

        // act
        var result = await authorizationService.AuthorizeAsync(user, httpContext, ReadPermission);

        // assert
        Assert.True(result.Succeeded);
    }

    [Theory]
    [InlineData("group")]
    [InlineData("customClaim")]
    [InlineData("custom-claim")]
    public async Task AuthorizeAsync_should_return_succeeded_true_when_using_a_custom_ClaimTypeContainingUserRole(string claimTypeContainingUserRole)
    {
        // arrange
        var authorizationService = AuthorizationServiceBuilder.CreateWithRbacAuthorization(options =>
        {
            options.UserRoleClaimType = claimTypeContainingUserRole;

            options.ConfigureRoles(roles => roles
                .Add(SupervisorRole).WithPermissions(ReadPermission));
        });

        var user = new TestUserBuilder()
            .SetUserId(TestUserId)
            .SetRoleClaimName(claimTypeContainingUserRole)
            .AddRole(SupervisorRole)
            .Build();

        // act
        var result = await authorizationService.AuthorizeAsync(user, httpContext, ReadPermission);

        // assert
        Assert.True(result.Succeeded);
    }

    [Theory]
    [InlineData(SupervisorRole, true)]
    [InlineData(ManagerRole, false)]
    public async Task AuthorizeAsync_should_return_succeeded_true_when_using_a_custom_UserRolesLocator(string userRole, bool expectedResult)
    {
        // arrange
        var authorizationService = AuthorizationServiceBuilder.CreateWithRbacAuthorization(
            options =>
            {
                options.ConfigureRoles(roles => roles
                    .Add(SupervisorRole).WithPermissions(ReadPermission));
            },
            services =>
            {
                services.AddSingleton<IUserRolesLocator>(new TestUserRolesLocator(userRole));
            });

        var user = new TestUserBuilder()
            .SetUserId(TestUserId)
            .Build();

        // act
        var result = await authorizationService.AuthorizeAsync(user, httpContext, ReadPermission);

        // assert
        Assert.Equal(expectedResult, result.Succeeded);
    }

    [Theory]
    [InlineData(ReadPermission, true)]
    [InlineData(WritePermission, false)]
    public async Task AuthorizeAsync_should_return_succeeded_true_when_using_a_custom_RoleConfigurationLocator(string rolePermission, bool expectedResult)
    {
        // arrange
        var authorizationService = AuthorizationServiceBuilder.CreateWithRbacAuthorization(
            options =>
            {
            },
            services =>
            {
                services.AddSingleton<IRoleConfigurationLocator>(new TestRoleConfigurationLocator(SupervisorRole, rolePermission));
            });

        var user = new TestUserBuilder()
            .SetUserId(TestUserId)
            .AddRole(SupervisorRole)
            .Build();

        // act
        var result = await authorizationService.AuthorizeAsync(user, httpContext, ReadPermission);

        // assert
        Assert.Equal(expectedResult, result.Succeeded);
    }
}