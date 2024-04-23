namespace RbacAuthorization.Locators;

using System.Collections.Immutable;
using System.Security.Claims;

public class ClaimsPrincipalUserRolesLocator : IUserRolesLocator
{
    private readonly string userRoleClaimName;

    public ClaimsPrincipalUserRolesLocator(string userRoleClaimName)
    {
        this.userRoleClaimName = userRoleClaimName ?? throw new ArgumentNullException(nameof(userRoleClaimName));
    }

    public Task<ImmutableList<Role>> GetUserRolesAsync(ClaimsPrincipal user)
    {
        ArgumentNullException.ThrowIfNull(user);

        return Task.FromResult(user.Claims
            .Where(claim => claim.Type == userRoleClaimName)
            .Select(c => Role.Parse(c.Value))
            .ToImmutableList());
    }
}

