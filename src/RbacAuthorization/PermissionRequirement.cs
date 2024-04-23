namespace RbacAuthorization;

using Microsoft.AspNetCore.Authorization;

public class PermissionRequirement : IAuthorizationRequirement
{
    public PermissionRequirement(string permission)
    {
        ArgumentNullException.ThrowIfNull(permission);

        Permission = permission;
    }

    public string Permission { get; }
}
