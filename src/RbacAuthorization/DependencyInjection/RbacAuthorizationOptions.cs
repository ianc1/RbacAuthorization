namespace RbacAuthorization.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

public class RbacAuthorizationOptions
{
    public RbacAuthorizationOptions(IServiceCollection services)
    {
        Services = services;
    }

    public IServiceCollection Services { get; }
}