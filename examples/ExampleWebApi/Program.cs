using ExampleWebApi.Services;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

using RbacAuthorization.DependencyInjection;
using RbacAuthorization.ExampleWebApi.Authorization;

using Swashbuckle.AspNetCore.SwaggerGen;

using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddRbacAuthorization(options =>
{
    options.AddClaimsPrincipalUserId(ClaimTypes.NameIdentifier);

    options.AddClaimsPrincipalUserRoles(ClaimTypes.Role);

    options.AddInMemoryRoleDefinitions(
        Roles.Admin,
        Roles.User,
        Roles.OrganizationAdmin,
        Roles.OrganizationUser,
        Roles.ProjectAdmin);
});

builder.Services.AddControllers();

builder.Services.AddSingleton<Repositories>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(ConfigureSwaggerSecurity);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

app.Run();

void ConfigureSwaggerSecurity(SwaggerGenOptions options)
{
    options.SupportNonNullableReferenceTypes();

    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        In = ParameterLocation.Header,
        Description = $"Refer to the {nameof(ExampleWebApi)} README.md for details on how to generate test tokens."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id =  JwtBearerDefaults.AuthenticationScheme
                }
            },
            []
        }
    });
}