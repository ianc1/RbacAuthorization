namespace RbacAuthorization;

using System.Security.Claims;

public static class ClaimsPrincipalExtensions
{
    public static IEnumerable<string> Roles(this ClaimsPrincipal user)
        => user.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);

    public static string EmailAddress(this ClaimsPrincipal user)
        => user.Claims.Where(c => c.Type == ClaimTypes.Email).First().Value;

    public static string UserId(this ClaimsPrincipal user)
        => user.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).First().Value;

    public static string Username(this ClaimsPrincipal user)
        => user.Claims.Where(c => c.Type == "preferred_username").First().Value;

    public static string GivenName(this ClaimsPrincipal user)
        => user.Claims.Where(c => c.Type == ClaimTypes.GivenName).First().Value;

    public static string Surname(this ClaimsPrincipal user)
        => user.Claims.Where(c => c.Type == ClaimTypes.Surname).First().Value;
}