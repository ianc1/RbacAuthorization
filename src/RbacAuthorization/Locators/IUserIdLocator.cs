namespace RbacAuthorization;

using System.Security.Claims;

public interface IUserIdLocator
{
    string? GetUserId(ClaimsPrincipal user);
}
