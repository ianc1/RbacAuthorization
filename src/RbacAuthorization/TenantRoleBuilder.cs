namespace RbacAuthorization;

public class TenantRoleBuilder
{
    private readonly RbacAuthorizationOptions rbacAuthorizationOptions;

    public TenantRoleBuilder(RbacAuthorizationOptions rbacAuthorizationOptions)
    {
        this.rbacAuthorizationOptions = rbacAuthorizationOptions ?? throw new ArgumentNullException(nameof(rbacAuthorizationOptions));
    }

    public bool IsTenantRole(string role)
        => !string.IsNullOrEmpty(rbacAuthorizationOptions.TenantRoleNameVariable)
            && role.Contains(rbacAuthorizationOptions.TenantRoleNameVariable, StringComparison.OrdinalIgnoreCase);

    public string Build(string tenantRoleTemplate, string requestedTenantId)
        => !string.IsNullOrEmpty(rbacAuthorizationOptions.TenantRoleNameVariable)
            ? tenantRoleTemplate.Replace(rbacAuthorizationOptions.TenantRoleNameVariable, requestedTenantId, StringComparison.OrdinalIgnoreCase)
            : throw new InvalidOperationException($"{nameof(RbacAuthorizationOptions.TenantRoleNameVariable)} not configured.");
}