namespace RbacAuthorization;

using System.Collections.Concurrent;

public class RoleConfigurationCache : IRoleConfigurationCache
{
    private readonly ConcurrentBag<RbacAuthorizationRoleConfiguration> cache = new ();

    private readonly IRoleConfigurationLocator permissionAssignmentsLocator;
    private readonly IRoleTenantIdVariableSubstitution roleTenantIdVariableSubstitution;

    public RoleConfigurationCache(IRoleConfigurationLocator permissionAssignmentsLocator, IRoleTenantIdVariableSubstitution roleTenantIdVariableSubstitution)
    {
        this.permissionAssignmentsLocator = permissionAssignmentsLocator ?? throw new ArgumentNullException(nameof(permissionAssignmentsLocator));
        this.roleTenantIdVariableSubstitution = roleTenantIdVariableSubstitution ?? throw new ArgumentNullException(nameof(roleTenantIdVariableSubstitution));
    }

    public async Task Reload()
    {
        var latestPermissionAssignments = await permissionAssignmentsLocator.GetRoleConfigurationsAsync();

        cache.Clear(); // Todo - Change to only update the assignments which have changed.

        foreach(var latestPermissionAssignment in latestPermissionAssignments)
        {
            cache.Add(latestPermissionAssignment);
        }
    }

    public async Task<IEnumerable<string>> GetRolesWithPermission(string permission, string? requiredTenantId)
    {
        if (cache.IsEmpty)
        {
            await Reload();
        }

        return cache.Where(assignment => HasPermission(assignment, permission, requiredTenantId))
            .Select(assignment => roleTenantIdVariableSubstitution.Substitute(assignment.Role, requiredTenantId));
    }

    private bool HasPermission(RbacAuthorizationRoleConfiguration assignment, string permission, string? requiredTenantId) =>
        assignment.Permissions.Contains(permission) &&
        (!roleTenantIdVariableSubstitution.RequiresSubstitution(assignment.Role) || !string.IsNullOrWhiteSpace(requiredTenantId));
}