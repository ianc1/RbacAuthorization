namespace RbacAuthorization.ExampleWebApi.Authorization;

public static class Permissions
{
    public const string UsersSearch = "Users.Search";
    public const string UsersRead = "Users.Read";

    public const string OrganizationsSearch = "Organizations.Search";
    public const string OrganizationsRead = "Organizations.Read";

    public const string ProjectsCreate = "Projects.Create";
    public const string ProjectsSearch = "Projects.Search";
    public const string ProjectsRead = "Projects.Read";
    public const string ProjectsDelete = "Projects.Delete";

    public const string TasksCreate = "Tasks.Create";
    public const string TasksSearch = "Tasks.Search";
    public const string TasksRead = "Tasks.Read";
    public const string TasksDelete = "Tasks.Delete";
}