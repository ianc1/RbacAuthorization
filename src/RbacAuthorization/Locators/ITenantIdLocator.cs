namespace RbacAuthorization;

using Microsoft.AspNetCore.Http;

public interface ITenantIdLocator
{
    public string? GetTenantId(HttpContext httpContext);
}
