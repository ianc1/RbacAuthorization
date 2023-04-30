namespace RbacAuthorization;

public interface IPolicy
{
    Task<IEnumerable<string>> GetAllPermissionsAsync();

    Task<IEnumerable<string>> GetRolesWithPermissionAsync(string permission);
}