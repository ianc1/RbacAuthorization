namespace RbacAuthorization;

public interface IRoleConfigurationLocator
{
    Task<IEnumerable<RbacAuthorizationRoleConfiguration>> GetRoleConfigurationsAsync();
}
