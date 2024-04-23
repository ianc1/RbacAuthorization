namespace RbacAuthorization.Locators;

using System.Collections.Immutable;
using System.Security.Claims;

public interface IUserRolesLocator
{
    Task<ImmutableList<Role>> GetUserRolesAsync(ClaimsPrincipal user);
}
