namespace RbacAuthorization;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using RbacAuthorization.Utilities;
using RbacAuthorization.Locators;

public class RbacAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IUserIdLocator userIdLocator;
    private readonly RbacAuthorizationService rbacAuthorizationService;
    private readonly ILogger<RbacAuthorizationHandler> logger;

    public RbacAuthorizationHandler(
        IUserIdLocator userIdLocator,
        RbacAuthorizationService rbacAuthorizationService,
        ILogger<RbacAuthorizationHandler> logger)
    {
        this.userIdLocator = userIdLocator ?? throw new ArgumentNullException(nameof(userIdLocator));
        this.rbacAuthorizationService = rbacAuthorizationService ?? throw new ArgumentNullException(nameof(rbacAuthorizationService));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
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
            logger.LogUnknownUserDeniedPermission(requirement.Permission);
            return;
        }

        var requestPath = httpContext.Request.Path;

        var result = await rbacAuthorizationService.HasPermission(context.User, requestPath, requirement.Permission);

        if (result.HasPermission)
        {
            logger.LogUserGrantedPermissionByRole(userId, requirement.Permission, result.UserRolesWithPermission);

            context.Succeed(requirement);
        }
        else
        {
            logger.LogUserDeniedPermission(userId, requirement.Permission, result.AllRolesWithPermission, result.UserRoles);
        }
    }
}