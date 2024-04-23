namespace RbacAuthorization;

using System.Collections.Immutable;
using System.Security.Claims;

using RbacAuthorization.Locators;

public class TestUserRolesLocator : IUserRolesLocator
{
    private readonly Role role;

    public TestUserRolesLocator(Role role)
    {
        this.role = role;
    }

    public async Task<ImmutableList<Role>> GetUserRolesAsync(ClaimsPrincipal user)
    {
        ArgumentNullException.ThrowIfNull(user);

        await Task.Delay(500);

        return [role];
    }
}

