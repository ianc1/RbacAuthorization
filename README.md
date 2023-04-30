# RbacAuthorization
A simple role based access control library for single and multi tenant applications.



## Single Tenant Application

The below task management application has two types of users, Supervisors and Assistants. Only supervisors can create and delete tasks while both can read and update the tasks. 

| Endpoint | Permission | Roles |
| --- | --- | --- |
| POST /tasks | Tasks.Create | MyApp.Supervisor |
| GET /tasks | Tasks.Read | MyApp.Assistant <br> MyApp.Supervisor |
| PUT /tasks/{taskId} | Tasks.Update | MyApp.Assistant <br> MyApp.Supervisor |
| DELETE /tasks/{taskId} | Tasks.Delete | MyApp.Supervisor |


### Configuration steps

1) Add the RbacAuthorization Nuget package.
```
dotnet add package RbacAuthorization
```

2) Define your permissions. These can use any format you like but typically include a resource and an action. For example:
```c#
public static class Permissions
{
    public const string TasksCreate = "Tasks.Create";
    public const string TasksRead = "Tasks.Read";
    public const string TasksUpdate = "Tasks.Update";
    public const string TasksDelete = "Tasks.Delete";
}
```

3) Define your roles. These can also use any format you like but typically include the app name and a job role. For example:
```c#
public static class Roles
{
    public const string Supervisor = "MyApp.Supervisor";
    public const string Assistant = "MyApp.Assistant";
}
```

4) Define a policy to map your roles to permissions. In the below example
the application has two types of users, Supervisors and Assistants. Only supervisors can create and delete tasks while
both can read and update the tasks.
```c#
builder.Services.AddRbacAuthorization(builder.Configuration, options =>
{
    options.Policy = new StaticPolicyBuilder()
        .AddRolePermissions(Roles.Supervisor, Permissions.TasksCreate, Permissions.TasksRead, Permissions.TasksUpdate, Permissions.TasksDelete)
        .AddRolePermissions(Roles.Assistant, Permissions.TasksRead, Permissions.TasksUpdate)
        .Build();
});
```

5) Assign the permissions to your controller actions using the standard authorize attribute:
```c#
app.MapGet("/tasks", [Authorize(Permissions.TasksRead)] () =>
{
    return Results.Ok(tasks.GetAll());
});
```

6) Configure your Identity Provider to include the relevant roles as `role` claims for your users. This typically involves creating a
group with the name of each role and assigning them to your users.

    - [Configuring roles in Active Directory](https://learn.microsoft.com/en-us/azure/active-directory/develop/howto-add-app-roles-in-azure-ad-apps#assign-users-and-groups-to-roles)



## Multi Tenant Application

This multi tenant example builds on top the single tenant example above. There are still two types of users, Supervisors and Assistants but this time they are per tenant to provide tenant isolation.

To avoid mapping the same roles for each tenant, multi tenant roles contain a placeholder for the tenant identifier. By default the placeholder is `$TenantId` but it can be changed to match what you call your tenant identifier. For example `$AccountName` or `$CompanyId`.

By default the library will obtain the tenant identifier from the request RouteData value named `TenantId`. You can also obtain the tenant identifier from a request header or subdomain if its not included in the path.

| Request | Permission | Roles |
| --- | --- | --- |
| POST /{TenantId}/tasks | Tasks.Create | MyApp.$TenantId.Supervisor |
| GET /{TenantId}/tasks | Tasks.Read | MyApp.$TenantId.Assistant <br> MyApp.$TenantId.Supervisor |
| PUT /{TenantId}/tasks/{taskId} | Tasks.Update | MyApp.$TenantId.Assistant <br> MyApp.$TenantId.Supervisor |
| DELETE /{TenantId}/tasks/{taskId} | Tasks.Delete | MyApp.$TenantId.Supervisor |


[View Source](https://github.com/ianc1/RbacAuthorization/tree/main/examples/ExampleWebApi)


### Configuration steps

1) Add the RbacAuthorization Nuget package.
```
dotnet add package RbacAuthorization
```

2) Define your permissions. These can use any format you like but typically include a resource and an action. For example:
```c#
public static class Permissions
{
    public const string TasksCreate = "Tasks.Create";
    public const string TasksRead = "Tasks.Read";
    public const string TasksUpdate = "Tasks.Update";
    public const string TasksDelete = "Tasks.Delete";
}
```

3) Define your roles. These can also use any format you like but typically include the app name, tenant identifier and a job role.
You can also include roles that span all tenants like a customer support role. For example:
```c#
public static class Roles
{
    public const string TenantSupervisor = "MyApp.$TenantId.Supervisor";
    public const string TenantAssistant = "MyApp.$TenantId.Assistant";
    public const string CustomerSupport = "MyApp.CustomerSupport";
}
```

4) Define a policy to map your roles to permissions. In the below example the application has three types of users, per tenant
Supervisors and Assistants users and application wide Customer Support staff. Only supervisors can create and delete tasks in their
tenant while both Supervisor and Assistants can read and update tasks in their tenant. Customer Support staff can read tasks in any tenant due
to their role not being scoped to a tenant with the $TenantId placeholder.
```c#
builder.Services.AddRbacAuthorization(builder.Configuration, options =>
{
    options.Policy = new StaticPolicyBuilder()
        .AddRolePermissions(Roles.TenantSupervisor, Permissions.TasksCreate, Permissions.TasksRead, Permissions.TasksUpdate, Permissions.TasksDelete)
        .AddRolePermissions(Roles.TenantAssistant, Permissions.TasksRead, Permissions.TasksUpdate)
        .AddRolePermissions(Roles.CustomerSupport, Permissions.TasksRead)
        .Build();
});
```

5) Assign the permissions to your controller actions using the standard authorize attribute:
```c#
app.MapGet("/tasks", [Authorize(Permissions.TasksRead)] () =>
{
    return Results.Ok(tasks.GetAll());
});
```

6) Configure your Identity Provider to include the relevant roles as `role` claims for your users. This typically involves creating a
group with the name of each role and assigning them to your users.

    - [Configuring roles in Active Directory](https://learn.microsoft.com/en-us/azure/active-directory/develop/howto-add-app-roles-in-azure-ad-apps#assign-users-and-groups-to-roles)
