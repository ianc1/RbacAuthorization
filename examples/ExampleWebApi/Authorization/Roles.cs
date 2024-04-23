namespace RbacAuthorization.ExampleWebApi.Authorization;

public static class Roles
{
    public static readonly RoleDefinition Admin = new(
        name: "Admin",
        permissions:
        [
            Permissions.UsersSearch,
            Permissions.UsersRead,

            Permissions.OrganizationsSearch,
            Permissions.OrganizationsRead,

            Permissions.ProjectsCreate,
            Permissions.ProjectsSearch,
            Permissions.ProjectsRead,
            Permissions.ProjectsDelete,

            Permissions.TasksCreate,
            Permissions.TasksSearch,
            Permissions.TasksRead,
            Permissions.TasksDelete,
        ]);

    public static readonly RoleDefinition User = new(
        name: "User",
        permissions:
        [
            Permissions.UsersRead,
        ],
        pathScope: "/users/me");

    public static readonly RoleDefinition OrganizationAdmin = new(
        name: "OrganizationAdmin",
        permissions:
        [
            Permissions.OrganizationsRead,

            Permissions.ProjectsCreate,
            Permissions.ProjectsSearch,
            Permissions.ProjectsRead,
            Permissions.ProjectsDelete,

            Permissions.TasksCreate,
            Permissions.TasksSearch,
            Permissions.TasksRead,
            Permissions.TasksDelete,
        ],
        pathScope: "/organizations/{OrganizationId}");

    public static readonly RoleDefinition OrganizationUser = new(
        name: "OrganizationUser",
        permissions:
        [
            Permissions.OrganizationsRead,

            Permissions.ProjectsSearch,
            Permissions.ProjectsRead,

            Permissions.TasksSearch,
            Permissions.TasksRead,

        ],
        pathScope: "/organizations/{OrganizationId}");

    public static readonly RoleDefinition ProjectAdmin = new(
        name: "ProjectAdmin",
        permissions:
        [
            Permissions.TasksCreate,
            Permissions.TasksDelete,
        ],
        pathScope: "/organizations/{OrganizationId}/projects/{ProjectId}");
}
