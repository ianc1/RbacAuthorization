namespace RbacAuthorization;

public record RbacAuthorizationResult(
    bool HasPermission,
    IEnumerable<Role> AllRolesWithPermission,
    IEnumerable<Role> UserRolesWithPermission,
    IEnumerable<Role> UserRoles);