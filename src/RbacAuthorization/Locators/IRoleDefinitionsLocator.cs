namespace RbacAuthorization.Locators;

using System.Collections.Immutable;

public interface IRoleDefinitionsLocator
{
    Task<ImmutableList<RoleDefinition>> GetRoleDefinitionsAsync();
}
