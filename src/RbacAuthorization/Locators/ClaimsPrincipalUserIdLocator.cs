namespace RbacAuthorization.Locators;

using System.Security.Claims;

public class ClaimsPrincipalUserIdLocator : IUserIdLocator
{
    private readonly string userIdClaimName;

    public ClaimsPrincipalUserIdLocator(string userIdClaimName)
    {
        this.userIdClaimName = userIdClaimName ?? throw new ArgumentNullException(nameof(userIdClaimName));
    }

    public string? GetUserId(ClaimsPrincipal user)
    {
        ArgumentNullException.ThrowIfNull(user);

        return user.Claims.Where(c => c.Type == userIdClaimName).FirstOrDefault()?.Value;
    }
}
