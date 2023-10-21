namespace RbacAuthorization;

using System.Security.Claims;
using RbacAuthorization.DependencyInjection;

public class UserIdClaimsPrincipalLocator : IUserIdLocator
{
    private readonly RbacAuthorizationOptions rbacAuthorizationOptions;

    public UserIdClaimsPrincipalLocator(RbacAuthorizationOptions rbacAuthorizationOptions)
    {
        this.rbacAuthorizationOptions = rbacAuthorizationOptions ?? throw new ArgumentNullException(nameof(rbacAuthorizationOptions));
    }

    public string? GetUserId(ClaimsPrincipal user)
    {
        ArgumentNullException.ThrowIfNull(user);

        return user.Claims.Where(c => c.Type == rbacAuthorizationOptions.UserIdClaimType).FirstOrDefault()?.Value;
    }
}
