# RbacAuthorization
A flexible Role Based Access Control library that's simple to setup and configure.

The library allows you to assign permissions to controller actions and then
group these permissions into roles which are then assigned to users.

The default implementation stores the role definitions in memory and retrieves
the user's id and roles from their token claims. However these can be retrieved
from any location. See [Customizing Role Retrieval].

Roles can be scoped to a particular request path to further restrict the assigned
permissions. See [Scoping Role Permissions].


### Basic Configuration

1) Add the RbacAuthorization Nuget package.
```
dotnet add package RbacAuthorization
```

2) Configure and assign permissions to you controller actions.

```c#
public static class Permissions
{
    public const string TasksCreate = "Tasks.Create";
    public const string TasksRead = "Tasks.Read";
}
```

```c#
[HttpPost]
[AuthorizePermission(Permissions.TasksCreate)]
public ActionResult<TaskDto> CreateTask(TaskCreateDto dto)
{
...
}

[HttpGet("{taskId}")]
[AuthorizePermission(Permissions.TasksRead)]
public ActionResult<TaskDto> GetTask(int taskId)
{
...
}
```

3) Configure your role definitions.

```c#
public static class Roles
{
    public static readonly RoleDefinition Admin = new(
        name: "Admin",
        permissions:
        [
            Permissions.TasksCreate,
            Permissions.TasksRead,
        ]);
}
```

```c#
builder.Services.AddRbacAuthorization(options =>
{
    options.AddClaimsPrincipalUserId(ClaimTypes.NameIdentifier);

    options.AddClaimsPrincipalUserRoles(ClaimTypes.Role);

    options.AddInMemoryRoleDefinitions(
        Roles.Admin);
});
```

4) Configure users with roles.

   Configure your Identity Provider to include the relevant roles as `role` claims for your users. This typically involves creating a
group with the name of each role and assigning them to your users.

   For example: [Configuring roles in Active Directory](https://learn.microsoft.com/en-us/azure/active-directory/develop/howto-add-app-roles-in-azure-ad-apps#assign-users-and-groups-to-roles)


### Scoping Role Permissions
Roles can be scoped to a particular request path to further restrict the assigned
permissions. The path scope can also include parameters to avoid having to define
multiple similar roles.

In the below example a role called `ProjectAdmin` has been scoped to the path
`/projects/{ProjectId}` which restricts its permissions to a particular project.

When the role is assigned to a user, it must also include the path scope and any
parameters must be replaced with values.

For example a user with the `ProjectAdmin:/projects/123` role would have the admin
permissions for only project `123`.

```c#
public static readonly RoleDefinition ProjectAdmin = new(
    name: "ProjectAdmin",
    permissions:
    [
        Permissions.TasksCreate,
        Permissions.TasksDelete,
    ],
    pathScope: "/projects/{ProjectId}");
```

### Customizing Role Retrieval

How role definitions and user roles are retrieved can be customized by implementing
a custom locator. The following are available:

- IRoleDefinitionsLocator
- IUserRolesLocator
- IUserIdLocator

A common scenario would be to retrieve the user's roles from a database instead
of from the user's token.

The custom locator will be called for each authorization.

The custom implementations should be registered as a singleton. For example:

```c#
options.Services.AddSingleton<IUserRolesLocator, MyCustomUserRolesLocator>();
```

### Example WebApi

An example WebApi showing the core functionality is available below: 

[View Source](https://github.com/ianc1/RbacAuthorization/tree/main/examples/ExampleWebApi)
