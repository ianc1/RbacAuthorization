namespace RbacAuthorization.ConfigureRoles;

public class RolePermissionsBuilder
{
    private readonly RolesConfigurationBuilder policyBuilder;
    private readonly string role;

    public RolePermissionsBuilder(RolesConfigurationBuilder policyBuilder, string role)
    {
        this.policyBuilder = policyBuilder ?? throw new ArgumentNullException(nameof(policyBuilder));
        this.role = role ?? throw new ArgumentNullException(nameof(role));
    }

    public RolesConfigurationBuilder WithPermissions(params string[] permissions)
    {
        return policyBuilder.AddRoleConfiguration(new RbacAuthorizationRoleConfiguration(role, permissions));
    }

    public RolesConfigurationBuilder WithPermission(string permission)
    {
        return WithPermissions(new[] { permission });
    }
}