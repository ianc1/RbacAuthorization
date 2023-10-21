namespace RbacAuthorization.Tests.TestHarness;

using System.Security.Claims;

using static RbacAuthorization.Tests.TestHarness.TestValues;

public class TestUserBuilder
{
    private IList<string> roles = new List<string>();

    private string roleClaimName = ClaimTypes.Role;

    private string userIdClaimName = ClaimTypes.NameIdentifier;

    private string? userId;

    public static ClaimsPrincipal ValidUser() =>
        new TestUserBuilder()
            .SetUserId(TestUserId)
            .AddRole(SupervisorRole)
            .Build();

    public TestUserBuilder SetUserId(string userId)
    {
        this.userId = userId;
        return this;
    }

    public TestUserBuilder SetUserIdClaimName(string userIdClaimName)
    {
        this.userIdClaimName = userIdClaimName;
        return this;
    }

    public TestUserBuilder AddRole(string role)
    {
        roles.Add(role);
        return this;
    }

    public TestUserBuilder SetRoleClaimName(string roleClaimName)
    {
        this.roleClaimName = roleClaimName;
        return this;
    }

    public ClaimsPrincipal Build()
    {
        var claims = new List<Claim>();

        if (userId != null)
        {
            claims.Add(new Claim(userIdClaimName, userId));
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(roleClaimName, role));
        }

        return new ClaimsPrincipal(new ClaimsIdentity(claims, "JwtBearer"));
    }
}
