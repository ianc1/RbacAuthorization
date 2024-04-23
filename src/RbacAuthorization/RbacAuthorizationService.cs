namespace RbacAuthorization;

using System.Collections.Immutable;
using System.Data;
using System.Security.Claims;

using Microsoft.AspNetCore.Http;

using RbacAuthorization.Locators;

public class RbacAuthorizationService
{
    private readonly IUserRolesLocator userRolesLocator;
    private readonly IRoleDefinitionsLocator roleDefinitionsLocator;

    public RbacAuthorizationService(IUserRolesLocator userRolesLocator, IRoleDefinitionsLocator roleDefinitionsLocator)
    {
        this.userRolesLocator = userRolesLocator ?? throw new ArgumentNullException(nameof(userRolesLocator));
        this.roleDefinitionsLocator = roleDefinitionsLocator ?? throw new ArgumentNullException(nameof(roleDefinitionsLocator));
    }

    public async Task<RbacAuthorizationResult> HasPermission(ClaimsPrincipal user, PathString requestPath, string requiredPermission)
    {
        var userRoles = await userRolesLocator.GetUserRolesAsync(user);

        var rolesWithPermission = await GetRolesWithPermission(requestPath, requiredPermission);

        var userRolesWithPermission = userRoles.Where(rolesWithPermission.Contains);

        return new RbacAuthorizationResult(
            HasPermission: userRolesWithPermission.Any(),
            AllRolesWithPermission: rolesWithPermission,
            UserRolesWithPermission: userRolesWithPermission,
            UserRoles: userRoles);
    }
    
    private async Task<List<Role>> GetRolesWithPermission(PathString requestPath, string permission)
    {
        var rolesWithPermission = new List<Role>();

        foreach (var roleDefinition in await roleDefinitionsLocator.GetRoleDefinitionsAsync())
        {
            if (roleDefinition.HasPermission(permission, requestPath, out var roleWithPermission))
            {
                rolesWithPermission.Add(roleWithPermission);
            }
        }

        return rolesWithPermission;
    }
}