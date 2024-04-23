namespace RbacAuthorization.Locators;

using System.Security.Claims;

public interface IUserIdLocator
{
    string? GetUserId(ClaimsPrincipal user);
}
