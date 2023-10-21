using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using RbacAuthorization.DependencyInjection;
using RbacAuthorization.ExampleWebApi;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddRbacAuthorization(options =>
{
    options.UserIdClaimType = ClaimTypes.NameIdentifier; // Default
    options.UserRoleClaimType = ClaimTypes.Role; // Default
    options.TenantIdVariableName = "TenantId"; // Default

    options.ConfigureRoles(roles => roles
        .Add(Roles.TenantSupervisor).WithPermissions(Permissions.TasksCreate, Permissions.TasksRead)
        .Add(Roles.TenantAssistant).WithPermission(Permissions.TasksRead)
        .Add(Roles.HelpDesk).WithPermission(Permissions.TasksRead));
});

var app = builder.Build();

app.UseAuthorization();

app.MapPost("/{TenantId}/tasks",
    [Authorize("unknown")]
    (string tenantId) =>
{
    var task = new Task(Id: $"{tenantId}-0", Title: "New todo task");

    return Results.Ok(task);
});

app.MapGet("/{TenantId}/tasks",
    [Authorize(Permissions.TasksRead)]
    (string tenantId) =>
{
    var tasks = new[] { new Task(Id: $"{tenantId}-1", "Existing todo task") };

    return Results.Ok(tasks);
});

app.Run();

record Task(string Id, string Title);