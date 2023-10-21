namespace RbacAuthorization.ConfigureRoles;

public class RolesConfigurationBuilder
{
    private readonly List<RbacAuthorizationRoleConfiguration> roleConfigurations = new();

    public RolesConfigurationBuilder AddRoleConfiguration(RbacAuthorizationRoleConfiguration permissionAssignment)
    {
        if (roleConfigurations.Any(existing => existing.Role.Equals(permissionAssignment.Role, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException($"Duplicate permission assignments detected for role '{permissionAssignment.Role}'.");
        }

        roleConfigurations.Add(permissionAssignment);

        return this;
    }

    public RolePermissionsBuilder Add(string name)
    {
        return new RolePermissionsBuilder(this, name);
    }

    public IEnumerable<RbacAuthorizationRoleConfiguration> Build() => roleConfigurations;
}