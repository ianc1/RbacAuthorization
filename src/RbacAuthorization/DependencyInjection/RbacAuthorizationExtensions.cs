namespace RbacAuthorization.DependencyInjection;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using RbacAuthorization.ConfigureRoles;
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
        services.AddSingleton<IRoleConfigurationCache, RoleConfigurationCache>();
        services.AddSingleton<IAuthorizationPolicyProvider, RbacAuthorizationPolicyProvider>();
        services.AddSingleton<IRoleTenantIdVariableSubstitution, RoleTenantIdVariableSubstitution>();

        // Register default locators
        services.AddSingleton<IUserIdLocator, UserIdClaimsPrincipalLocator>();
        services.AddSingleton<IUserRolesLocator, UserRolesClaimsPrincipalLocator>();
        services.AddSingleton<ITenantIdLocator, TenantIdRouteDataLocator>();
    }

    public static void ConfigureRoles(
        this RbacAuthorizationOptions options,
        Action<RolesConfigurationBuilder> rolesAction)
    {
        var builder = new RolesConfigurationBuilder();

        rolesAction.Invoke(builder);

        options.Services.AddSingleton<IRoleConfigurationLocator>(new StaticRoleConfigurationLocator(builder.Build()));
    }
}