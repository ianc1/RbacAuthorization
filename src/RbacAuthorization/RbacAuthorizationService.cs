namespace RbacAuthorization;

using System.Security.Claims;

public class RbacAuthorizationService
{
    private readonly IUserRolesLocator userRolesLocator;
    private readonly IRoleConfigurationCache permissionAssignmentsCache;

    public RbacAuthorizationService(IUserRolesLocator userRolesLocator, IRoleConfigurationCache permissionAssignmentsCache)
    {
        this.userRolesLocator = userRolesLocator ?? throw new ArgumentNullException(nameof(userRolesLocator));
        this.permissionAssignmentsCache = permissionAssignmentsCache ?? throw new ArgumentNullException(nameof(permissionAssignmentsCache));
    }

    public async Task<RbacAuthorizationResult> HasPermission(ClaimsPrincipal user, string requiredPermission, string? requiredTenantId = null)
    {
        var userRoles = await userRolesLocator.GetUserRolesAsync(user);

        var rolesWithPermission = await permissionAssignmentsCache.GetRolesWithPermission(requiredPermission, requiredTenantId);

        var userRolesWithPermission = rolesWithPermission.Where(requiredRole => userRoles.Any(role => role.Equals(requiredRole, StringComparison.OrdinalIgnoreCase)));

        return new RbacAuthorizationResult(
            HasPermission: userRolesWithPermission.Any(),
            AllRolesWithPermission: rolesWithPermission,
            UserRolesWithPermission: userRolesWithPermission,
            UserRoles: userRoles);
    }
}