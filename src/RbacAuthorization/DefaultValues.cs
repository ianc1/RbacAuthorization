using System.Security.Claims;

namespace RbacAuthorization;

public class DefaultValues
{
    public const string DefaultTenantIdVariableName = "TenantId";

    public const string DefaultClaimTypeContainingUserId = ClaimTypes.NameIdentifier;

    public const string DefaultClaimTypeContainingUserRole = ClaimTypes.Role;
}
