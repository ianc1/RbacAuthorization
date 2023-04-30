namespace RbacAuthorization;

using System.Security.Claims;

public class RbacAuthorizationService
{
    private readonly RbacAuthorizationOptions rbacAuthorizationOptions;
    private readonly TenantRoleBuilder tenantRoleBuilder;

    public RbacAuthorizationService(RbacAuthorizationOptions rbacAuthorizationOptions, TenantRoleBuilder tenantRoleBuilder)
    {
        this.rbacAuthorizationOptions = rbacAuthorizationOptions ?? throw new ArgumentNullException(nameof(rbacAuthorizationOptions));
        this.tenantRoleBuilder = tenantRoleBuilder ?? throw new ArgumentNullException(nameof(tenantRoleBuilder));
    }

    public async Task<RbacAuthorizationResult> HasPermission(
        ClaimsPrincipal user,
        string requiredPermission,
        string? requiredTenantId = null)
    {
        var userRoles = user.Roles();

        if (rbacAuthorizationOptions.Policy == null)
        {
            throw new InvalidOperationException($"{nameof(RbacAuthorizationOptions.Policy)} not configured.");
        }

        var rolesWithPermission = (await rbacAuthorizationOptions.Policy!.GetRolesWithPermissionAsync(requiredPermission))
            .Where(role => !tenantRoleBuilder.IsTenantRole(role) || !string.IsNullOrWhiteSpace(requiredTenantId))
            .Select(role => tenantRoleBuilder.IsTenantRole(role) ? tenantRoleBuilder.Build(role, requiredTenantId!) : role);

        var userRolesWithPermission = rolesWithPermission.Where(requiredRole => userRoles.Any(role => role.Equals(requiredRole, StringComparison.OrdinalIgnoreCase)));

        return new RbacAuthorizationResult(
            HasPermission: userRolesWithPermission.Any(),
            AllRolesWithPermission: rolesWithPermission,
            UserRolesWithPermission: userRolesWithPermission,
            UserRoles: userRoles);
    }
}