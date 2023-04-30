namespace RbacAuthorization;

public record RbacAuthorizationResult(
    bool HasPermission,
    IEnumerable<string> AllRolesWithPermission,
    IEnumerable<string> UserRolesWithPermission,
    IEnumerable<string> UserRoles);