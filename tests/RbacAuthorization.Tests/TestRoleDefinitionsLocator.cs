namespace RbacAuthorization.ConfigureRoles;

using System.Collections.Immutable;

using RbacAuthorization.Locators;

using static RbacAuthorization.Tests.TestHarness.TestValues;

public class TestRoleDefinitionsLocator : IRoleDefinitionsLocator
{
    private readonly string roleName;
    private readonly string permission;

    public TestRoleDefinitionsLocator(string roleName, string permission)
    {
        this.roleName = roleName;
        this.permission = permission;
    }

    public async Task<ImmutableList<RoleDefinition>> GetRoleDefinitionsAsync()
    {
        await Task.Delay(500);

        return
        [
            new RoleDefinition(
                name: roleName,
                permissions:
                [
                    permission,
                ]),
        ];
    }
}
