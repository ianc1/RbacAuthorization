namespace RbacAuthorization.Locators;

using System.Collections.Immutable;

public class InMemoryRoleDefinitionsLocator : IRoleDefinitionsLocator
{
    private readonly ImmutableList<RoleDefinition> roleDefinitions;

    public InMemoryRoleDefinitionsLocator(IEnumerable<RoleDefinition> roleDefinitions)
    {
        this.roleDefinitions = roleDefinitions?.ToImmutableList() ?? throw new ArgumentNullException(nameof(roleDefinitions));
    }

    public Task<ImmutableList<RoleDefinition>> GetRoleDefinitionsAsync()
    {
        return Task.FromResult(roleDefinitions);
    }
}
