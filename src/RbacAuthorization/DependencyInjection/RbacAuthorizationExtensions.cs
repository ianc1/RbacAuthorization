namespace RbacAuthorization.DependencyInjection;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using RbacAuthorization.Locators;

public static class RbacAuthorizationExtensions
{
    public static void AddRbacAuthorization(
        this IServiceCollection services,
        Action<RbacAuthorizationOptions> optionsAction)
    {
        services.AddAuthorization();

        var rbacAuthorizationOptions = new RbacAuthorizationOptions(services);
        optionsAction(rbacAuthorizationOptions);
        services.AddSingleton(rbacAuthorizationOptions);

        services.AddSingleton<RbacAuthorizationService>();
        services.AddSingleton<IAuthorizationHandler, RbacAuthorizationHandler>();
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
    }

    public static void AddClaimsPrincipalUserId(
        this RbacAuthorizationOptions options,
        string userIdClaimName)
    {
        options.Services.AddSingleton<IUserIdLocator>(new ClaimsPrincipalUserIdLocator(userIdClaimName));
    }

    public static void AddClaimsPrincipalUserRoles(
        this RbacAuthorizationOptions options,
        string userRoleClaimName)
    {
        options.Services.AddSingleton<IUserRolesLocator>(new ClaimsPrincipalUserRolesLocator(userRoleClaimName));
    }

    public static void AddInMemoryRoleDefinitions(
        this RbacAuthorizationOptions options,
        params RoleDefinition[] roleDefinitions)
    {
        options.Services.AddSingleton<IRoleDefinitionsLocator>(new InMemoryRoleDefinitionsLocator(roleDefinitions));
    }
}