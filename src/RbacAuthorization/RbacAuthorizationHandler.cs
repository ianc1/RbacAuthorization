namespace RbacAuthorization;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

public class RbacAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement>
{
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly RbacAuthorizationOptions rbacAuthorizationOptions;
    private readonly RbacAuthorizationService rbacAuthorizationService;
    private readonly ILogger<RbacAuthorizationHandler> logger;

    public RbacAuthorizationHandler(
        IHttpContextAccessor httpContextAccessor,
        RbacAuthorizationOptions rbacAuthorizationOptions,
        RbacAuthorizationService rbacAuthorizationService,
        ILogger<RbacAuthorizationHandler> logger)
    {
        this.httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        this.rbacAuthorizationOptions = rbacAuthorizationOptions ?? throw new ArgumentNullException(nameof(rbacAuthorizationOptions));
        this.rbacAuthorizationService = rbacAuthorizationService ?? throw new ArgumentNullException(nameof(rbacAuthorizationService));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement)
    {   
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(requirement);

        if (context.User.Identity == null || !context.User.Identity.IsAuthenticated)
        {
            return;
        }

        var requestedTenantId = rbacAuthorizationOptions.TenantIdLocator?.GetTenantId(httpContextAccessor);

        var result = await rbacAuthorizationService.HasPermission(context.User, requirement.Name, requestedTenantId);

        if (result.HasPermission)
        {
            logger.LogUserGrantedPermissionByRole(context.User.UserId(), requirement.Name, result.UserRolesWithPermission);

            context.Succeed(requirement);
        }
        else
        {
            logger.LogUserDeniedPermission(context.User.UserId(), requirement.Name, result.AllRolesWithPermission, result.UserRoles);
        }
    }


}