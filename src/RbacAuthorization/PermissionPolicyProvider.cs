namespace RbacAuthorization;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

public class PermissionPolicyProvider : IAuthorizationPolicyProvider
{
    public const string PolicyPrefix = "PermissionPolicy";
    public const char PolicyPrefixSeparator = ':';

    private readonly DefaultAuthorizationPolicyProvider defaultPolicyProvider;

    public PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
    {
        defaultPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() =>
        defaultPolicyProvider.GetDefaultPolicyAsync();
    
    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() =>
        defaultPolicyProvider.GetFallbackPolicyAsync();

    public async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        ArgumentNullException.ThrowIfNull(policyName);

        var components = policyName.Split(PolicyPrefixSeparator, 2);
        var prefix = components[0];
        var permission = components.Length == 2 ? components[1] : null;

        if (prefix != PolicyPrefix)
        {
            return await defaultPolicyProvider.GetPolicyAsync(policyName);
        }

        var policy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme);

        policy.AddRequirements(new PermissionRequirement(permission!));

        return policy.Build();
    }
}