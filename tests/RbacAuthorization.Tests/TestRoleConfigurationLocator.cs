namespace RbacAuthorization.ConfigureRoles;

using static RbacAuthorization.Tests.TestHarness.TestValues;

public class TestRoleConfigurationLocator : IRoleConfigurationLocator
{
    private readonly string role;
    private readonly string permission;

    public TestRoleConfigurationLocator(string role, string permission)
    {
        this.role = role;
        this.permission = permission;
    }

    public async Task<IEnumerable<RbacAuthorizationRoleConfiguration>> GetRoleConfigurationsAsync()
    {
        await Task.Delay(500);

        return new[]
        { 
            new RbacAuthorizationRoleConfiguration(
                Role: SupervisorRole,
                Permissions: new[]
                {
                    permission,
                }),
        };
    }
}
