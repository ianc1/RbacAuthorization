namespace RbacAuthorization.Utilities;

using Microsoft.Extensions.Logging;

internal static partial class LogMessage
{
    [LoggerMessage(EventId = 2, Level = LogLevel.Warning,
        Message = "User denied permission '{permission}' due to missing userId.")]
    public static partial void LogUnknownUserDeniedPermission(this ILogger logger, string permission);

    [LoggerMessage(EventId = 0, Level = LogLevel.Information,
        Message = "User '{userId}' granted permission '{permission}' by roles [{userRoles}].")]
    public static partial void LogUserGrantedPermissionByRole(this ILogger logger, string userId, string permission, IEnumerable<Role> userRoles);

    [LoggerMessage(EventId = 1, Level = LogLevel.Warning,
        Message = "User '{userId}' denied permission '{permission}' due to not having any of the required roles [{requiredRoles}]. User roles [{userRoles}].")]
    public static partial void LogUserDeniedPermission(this ILogger logger, string userId, string permission, IEnumerable<Role> requiredRoles, IEnumerable<Role> userRoles);
}