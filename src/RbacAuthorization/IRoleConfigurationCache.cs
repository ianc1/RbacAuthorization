namespace RbacAuthorization;

public interface IRoleConfigurationCache
{
    Task Reload();

    Task<IEnumerable<string>> GetRolesWithPermission(string permission, string? requiredTenantId);
}