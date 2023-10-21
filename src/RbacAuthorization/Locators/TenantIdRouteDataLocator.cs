namespace RbacAuthorization;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using RbacAuthorization.DependencyInjection;

public class TenantIdRouteDataLocator : ITenantIdLocator
{
    private readonly RbacAuthorizationOptions rbacAuthorizationOptions;

    public TenantIdRouteDataLocator(RbacAuthorizationOptions rbacAuthorizationOptions)
    {
        this.rbacAuthorizationOptions = rbacAuthorizationOptions ?? throw new ArgumentNullException(nameof(rbacAuthorizationOptions));
    }
    
    public string? GetTenantId(HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);
        ArgumentNullException.ThrowIfNull(rbacAuthorizationOptions.TenantIdVariableName);

        return httpContext.GetRouteData().Values[rbacAuthorizationOptions.TenantIdVariableName]?.ToString(); 
    }
}
