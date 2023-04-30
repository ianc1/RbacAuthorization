namespace RbacAuthorization;

public class StaticPolicyBuilder
{
    private readonly Dictionary<string, IEnumerable<string>> allRolePermissions = new Dictionary<string, IEnumerable<string>>();

    public StaticPolicyBuilder AddRolePermissions(string role, params string[] permissions)
    {
        allRolePermissions.Add(role, permissions);

        return this;
    }

    public IPolicy Build() => new StaticPolicy(allRolePermissions);
}