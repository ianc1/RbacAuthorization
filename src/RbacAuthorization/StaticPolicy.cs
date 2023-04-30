namespace RbacAuthorization;

public class StaticPolicy : IPolicy
{
    private readonly Dictionary<string, IEnumerable<string>> allRolePermissions;

    public StaticPolicy(Dictionary<string, IEnumerable<string>> allRolePermissions)
    {
        this.allRolePermissions = allRolePermissions;
    }

    public Task<IEnumerable<string>> GetAllPermissionsAsync() => Task.FromResult(
        allRolePermissions.Values.SelectMany(p => p).Distinct());

    public Task<IEnumerable<string>> GetRolesWithPermissionAsync(string permission) => Task.FromResult(
        allRolePermissions.Where(keyPair => keyPair.Value.Any(p => p.Equals(permission, StringComparison.OrdinalIgnoreCase)))
            .Select(keyPair => keyPair.Key));
}