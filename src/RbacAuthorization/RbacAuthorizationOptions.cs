namespace RbacAuthorization;

public class RbacAuthorizationOptions
{
    public IPolicy? Policy { get; set; }

    public string? TenantRoleNameVariable { get; set; }

    public ITenantIdLocator? TenantIdLocator { get; set; }
}