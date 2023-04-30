namespace RbacAuthorization;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

public static class RbacAuthorizationExtensions
{
    public static void AddRbacAuthorization(
        this IServiceCollection services,
        Action<RbacAuthorizationOptions> optionsAction)
    {   
        services.AddSingleton<RbacAuthorizationService>();   
        services.AddSingleton<IAuthorizationHandler, RbacAuthorizationHandler>();

        var rbacAuthorizationOptions = new RbacAuthorizationOptions();

        optionsAction(rbacAuthorizationOptions);

        ArgumentNullException.ThrowIfNull(rbacAuthorizationOptions.Policy);

        services.AddAuthorization(options =>
        {
            var permissions = rbacAuthorizationOptions.Policy.GetAllPermissionsAsync();

            permissions.Wait(); // Block Startup until permissions are loaded.

            AddPermissionPolicies(options, permissions.Result);
        });

        services.AddSingleton(rbacAuthorizationOptions);
        services.AddSingleton<TenantRoleBuilder>();
    }

    private static void AddPermissionPolicies(AuthorizationOptions options, IEnumerable<string> allPermissions)
    {
        foreach(var permission in allPermissions)
        {
            options.AddPolicy(permission, policy => policy.Requirements.Add(new OperationAuthorizationRequirement{ Name = permission }));    
        }
    }
}