#pragma warning disable CA1852

using Microsoft.AspNetCore.Authorization;
using RbacAuthorization;
using RbacAuthorization.ExampleWebApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthorization();
builder.Services.AddAuthentication("Bearer").AddJwtBearer();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddRbacAuthorization(options =>
{
    options.TenantRoleNameVariable = "$TenantId";
    options.TenantIdLocator = new RouteDataTenantIdLocator(routeDataName: "TenantId");

    options.Policy = new StaticPolicyBuilder()
        .AddRolePermissions(Roles.TenantSupervisor, Permissions.TasksCreate, Permissions.TasksRead)
        .AddRolePermissions(Roles.TenantAssistant, Permissions.TasksRead)
        .AddRolePermissions(Roles.HelpDesk, Permissions.TasksRead)
        .Build();
});

var app = builder.Build();

app.UseAuthorization();

app.MapPost("/{TenantId}/tasks",
    [Authorize(Permissions.TasksCreate)]
    (string TenantId) =>
{
    var task = new Task(Id: $"{TenantId}-0", Title: "New todo task");

    return Results.Ok(task);
});

app.MapGet("/{TenantId}/tasks",
    [Authorize(Permissions.TasksRead)]
    (string TenantId) =>
{
    var tasks = new[] { new Task(Id: $"{TenantId}-1", "Existing todo task") };

    return Results.Ok(tasks);
});

app.Run();

record Task(string Id, string Title);