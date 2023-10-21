namespace RbacAuthorization;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

public class RbacAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement>
{
    private readonly IUserIdLocator userIdLocator;
    private readonly ITenantIdLocator tenantIdLocator;
    private readonly RbacAuthorizationService rbacAuthorizationService;
    private readonly ILogger<RbacAuthorizationHandler> logger;

    public RbacAuthorizationHandler(
        IUserIdLocator userIdLocator,
        ITenantIdLocator tenantIdLocator,
        RbacAuthorizationService rbacAuthorizationService,
        ILogger<RbacAuthorizationHandler> logger)
    {
        this.userIdLocator = userIdLocator ?? throw new ArgumentNullException(nameof(userIdLocator));
        this.tenantIdLocator = tenantIdLocator ?? throw new ArgumentNullException(nameof(tenantIdLocator));
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

        if (context.Resource is not HttpContext httpContext)
        {
            return;
        }

        var userId = userIdLocator.GetUserId(context.User);

        if (userId == null)
        {
            logger.LogUnknownUserDeniedPermission(requirement.Name);
            return;
        }

        var requestedTenantId = tenantIdLocator.GetTenantId(httpContext);

        var result = await rbacAuthorizationService.HasPermission(context.User, requirement.Name, requestedTenantId);

        if (result.HasPermission)
        {
            logger.LogUserGrantedPermissionByRole(userId, requirement.Name, result.UserRolesWithPermission);

            context.Succeed(requirement);
        }
        else
        {
            logger.LogUserDeniedPermission(userId, requirement.Name, result.AllRolesWithPermission, result.UserRoles);
        }
    }
}