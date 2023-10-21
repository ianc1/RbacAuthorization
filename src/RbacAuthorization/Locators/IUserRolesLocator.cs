namespace RbacAuthorization;

using System.Security.Claims;

public interface IUserRolesLocator
{
    Task<IEnumerable<string>> GetUserRolesAsync(ClaimsPrincipal user);
}
