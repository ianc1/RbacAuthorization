namespace RbacAuthorization;

using System.Security.Claims;
using RbacAuthorization.DependencyInjection;

using static RbacAuthorization.Tests.TestHarness.TestValues;

public class TestUserRolesLocator : IUserRolesLocator
{
    private readonly string role;

    public TestUserRolesLocator(string role)
    {
        this.role = role;
    }

    public async Task<IEnumerable<string>> GetUserRolesAsync(ClaimsPrincipal user)
    {
        ArgumentNullException.ThrowIfNull(user);

        await Task.Delay(500);

        return new[] { role };
    }
}

