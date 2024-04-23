namespace RbacAuthorization;

using Microsoft.AspNetCore.Authorization;

public class AuthorizePermissionAttribute : AuthorizeAttribute
{
    public AuthorizePermissionAttribute(string permission)
    {
        ArgumentNullException.ThrowIfNull(permission);

        Policy = $"{PermissionPolicyProvider.PolicyPrefix}{PermissionPolicyProvider.PolicyPrefixSeparator}{permission}";
        Permission = permission;
    }

    public string Permission { get; }
}
