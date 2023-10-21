namespace RbacAuthorization;

using System.Security.Claims;
using RbacAuthorization.DependencyInjection;

public class UserRolesClaimsPrincipalLocator : IUserRolesLocator
{
    private readonly RbacAuthorizationOptions rbacAuthorizationOptions;

    public UserRolesClaimsPrincipalLocator(RbacAuthorizationOptions rbacAuthorizationOptions)
    {
        this.rbacAuthorizationOptions = rbacAuthorizationOptions ?? throw new ArgumentNullException(nameof(rbacAuthorizationOptions));
    }

    public Task<IEnumerable<string>> GetUserRolesAsync(ClaimsPrincipal user)
    {
        ArgumentNullException.ThrowIfNull(user);

        return Task.FromResult(user.Claims.Where(c => c.Type == rbacAuthorizationOptions.UserRoleClaimType).Select(c => c.Value));
    }
}

