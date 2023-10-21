namespace RbacAuthorization.Locators;

public class StaticRoleConfigurationLocator : IRoleConfigurationLocator
{
    private readonly IList<RbacAuthorizationRoleConfiguration> roleConfigurations;

    public StaticRoleConfigurationLocator(IEnumerable<RbacAuthorizationRoleConfiguration> roleConfigurations)
    {
        this.roleConfigurations = roleConfigurations?.ToList() ?? throw new ArgumentNullException(nameof(roleConfigurations));
    }

    public Task<IEnumerable<RbacAuthorizationRoleConfiguration>> GetRoleConfigurationsAsync()
    {
        return Task.FromResult((IEnumerable<RbacAuthorizationRoleConfiguration>)roleConfigurations);
    }
}
