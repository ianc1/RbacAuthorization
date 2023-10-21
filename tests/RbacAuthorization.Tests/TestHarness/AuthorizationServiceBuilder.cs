namespace RbacAuthorization.Tests.TestHarness;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using RbacAuthorization.DependencyInjection;

public class AuthorizationServiceBuilder
{
    public static IAuthorizationService CreateWithRbacAuthorization(
        Action<RbacAuthorizationOptions>? rbacAuthorizationOptionsAction = null,
        Action<IServiceCollection>? servicesAction = null)
    {
        var services = new ServiceCollection();

        services.AddAuthorization();
        services.AddLogging();
        services.AddOptions();

        services.AddRbacAuthorization(options =>
        {
            rbacAuthorizationOptionsAction?.Invoke(options);
        });

        servicesAction?.Invoke(services);

        return services.BuildServiceProvider().GetRequiredService<IAuthorizationService>();
    }
}
