namespace RbacAuthorization;

public interface IRoleTenantIdVariableSubstitution
{
    bool RequiresSubstitution(string role);

    string Substitute(string role, string? tenantId);
}
