namespace RbacAuthorization.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

public class RbacAuthorizationOptions
{
    public RbacAuthorizationOptions(IServiceCollection services)
    {
        Services = services;
    }

    public string UserIdClaimType { get; set; } = ClaimTypes.NameIdentifier;

    public string UserRoleClaimType { get; set; } = ClaimTypes.Role;

    public string TenantIdVariableName { get; set; } = DefaultValues.DefaultTenantIdVariableName;

    public IServiceCollection Services { get; }
}