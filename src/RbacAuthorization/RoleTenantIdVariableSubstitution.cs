using RbacAuthorization.DependencyInjection;

namespace RbacAuthorization;

public class RoleTenantIdVariableSubstitution : IRoleTenantIdVariableSubstitution
{
    private readonly string tenantIdVariable;

    public RoleTenantIdVariableSubstitution(RbacAuthorizationOptions rbacAuthorizationOptions)
    {
        ArgumentNullException.ThrowIfNull(rbacAuthorizationOptions);

        if (rbacAuthorizationOptions.TenantIdVariableName == null)
        {
            throw new InvalidOperationException($"{nameof(RbacAuthorizationOptions)}.{nameof(RbacAuthorizationOptions.TenantIdVariableName)} is not configured.");
        }

        tenantIdVariable = $"${rbacAuthorizationOptions.TenantIdVariableName}";
    }

    public bool RequiresSubstitution(string role) => role.Contains(tenantIdVariable, StringComparison.OrdinalIgnoreCase);

    public string Substitute(string role, string? tenantId)
    {
        ArgumentNullException.ThrowIfNull(role);

        return RequiresSubstitution(role) && !string.IsNullOrWhiteSpace(tenantId)
            ? role.Replace(tenantIdVariable, tenantId, StringComparison.OrdinalIgnoreCase)
            : role;
    }
}
