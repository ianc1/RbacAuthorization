namespace RbacAuthorization;

public record RbacAuthorizationRoleConfiguration(
    string Role,
    IEnumerable<string> Permissions);
