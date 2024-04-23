namespace RbacAuthorization.Tests;

using System.Threading.Tasks;

using FluentAssertions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.Options;

public class RbacAuthorizationPolicyProviderTests
{
    private readonly AuthorizationOptions options = new();
    private readonly PermissionPolicyProvider provider;

    public RbacAuthorizationPolicyProviderTests()
    {
        provider = new PermissionPolicyProvider(Options.Create(options));
    }

    [Fact]
    public async Task GetDefaultPolicyAsync_should_return_the_DenyAnonymousAuthorizationRequirement()
    {
        // arrange / act
        var defaultPolicy = await provider.GetDefaultPolicyAsync();

        // assert
        defaultPolicy.Should().NotBeNull();
        defaultPolicy.Requirements.Count.Should().Be(1);
        defaultPolicy.Requirements[0].Should().BeAssignableTo<DenyAnonymousAuthorizationRequirement>();
    }

    [Fact]
    public async Task GetFallbackPolicyAsync_should_return_null()
    {
        // arrange / act
        var fallbackPolicy = await provider.GetFallbackPolicyAsync();

        // assert
        fallbackPolicy.Should().BeNull();
    }

    [Fact]
    public async Task GetPolicyAsync_should_throw_when_the_policyName_parameter_is_null()
    {
        // arrange / act
        var act = async () => await provider.GetPolicyAsync(policyName: null!);

        // assert
        (await act.Should().ThrowAsync<ArgumentNullException>())
            .WithParameterName("policyName");
    }

    [Theory]
    [InlineData("PermissionPolicy:Read", "Read")]
    [InlineData("PermissionPolicy:Update", "Update")]
    public async Task GetPolicyAsync_should_return_the_permission_as_a_requirement(string policyName, string permission)
    {
        // arrange / act
        var policy = await provider.GetPolicyAsync(policyName);

        // assert
        policy.Should().NotBeNull();
        policy!.Requirements.Count.Should().Be(1);
        policy.Requirements[0].Should().BeOfType<PermissionRequirement>()
            .Which.Permission.Should().Be(permission);
    }

    [Fact]
    public async Task GetPolicyAsync_should_return_null_when_the_requested_policy_name_does_not_contain_the_permission_policy_prefix()
    {
        // arrange / act
        var policy = await provider.GetPolicyAsync("unknown-policy");

        // assert
        policy.Should().BeNull();
    }

    [Fact]
    public async Task GetPolicyAsync_should_allow_the_use_of_non_permission_policies()
    {
        // arrange
        var claimsPolicyName = "EmployeeOnly";
        var requiredClaim = "EmployeeNumber";

        options.AddPolicy(claimsPolicyName, policy =>
            policy.RequireClaim(requiredClaim));

        // act
        var policy = await provider.GetPolicyAsync(claimsPolicyName);

        // assert
        policy.Should().NotBeNull();
        policy!.Requirements.Count.Should().Be(1);
        policy.Requirements[0].Should().BeAssignableTo<ClaimsAuthorizationRequirement>()
            .Which.ClaimType.Should().Be(requiredClaim);
    }
}
