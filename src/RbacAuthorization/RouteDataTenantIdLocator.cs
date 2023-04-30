namespace RbacAuthorization;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

public class RouteDataTenantIdLocator : ITenantIdLocator
{
    private readonly string routeDataName;

    public RouteDataTenantIdLocator(string routeDataName)
    {
        this.routeDataName = routeDataName;
    }
    
    public string? GetTenantId(IHttpContextAccessor httpContextAccessor) =>
        httpContextAccessor.HttpContext?.GetRouteData().Values[routeDataName]?.ToString(); 
}

