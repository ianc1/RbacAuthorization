namespace RbacAuthorization;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

using Microsoft.AspNetCore.Http;
using RbacAuthorization.Internal;
using RbacAuthorization.Utilities;

public class RoleDefinition : ValueObject
{
    private readonly PathScopeDefinition? pathScopeDefinition;

    public RoleDefinition(string name, IEnumerable<string> permissions, string? pathScope = null)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(permissions);

        Name = ThrowIfInvalidName(name);
        Permissions = ThrowIfInvalidPermissions(permissions.ToImmutableList());
        pathScopeDefinition = pathScope is null ? null : new PathScopeDefinition(pathScope);
    }

    public string Name { get; }

    public ImmutableList<string> Permissions { get; }

    public string? PathScope => pathScopeDefinition?.Value;

    public Role CreateRole(params string[] pathScopeParameters)
    {
        if (pathScopeDefinition is null && pathScopeParameters.Length > 0)
        {
            throw new ArgumentException($"RoleDefinition does not contain a PathScope so PathScopeParameters should not be supplied.");
        }

        return pathScopeDefinition is null
            ? new Role(Name)
            : new Role(Name, pathScopeDefinition.ToPathScope(pathScopeParameters).Value);
    }

    public bool HasPermission(string requestPermission, PathString requestPath, [NotNullWhen(true)] out Role? requiredRole)
    {
        ArgumentNullException.ThrowIfNull(requestPermission);
        ArgumentNullException.ThrowIfNull(requestPath);

        if (!Permissions.Contains(requestPermission))
        {
            requiredRole = null;
            return false;
        }

        if (pathScopeDefinition is null)
        {
            requiredRole = new Role(Name);
            return true;
        }

        var isPathScopeMatch = pathScopeDefinition.MatchesRequestPath(requestPath, out var requiredPathScope);

        if (!isPathScopeMatch)
        {
            requiredRole = null;
            return false;
        }

        requiredRole = new Role(Name, requiredPathScope!.Value);
        return true;
    }

    public override string ToString()
    {
        return PathScope is null ? Name : $"{Name}:{PathScope}";
    }

    protected override IEnumerable<IComparable?> GetEqualityComponents()
    {
        yield return Name;

        foreach (var permission in Permissions)
        {
            yield return permission;
        }
        
        yield return pathScopeDefinition;
    }

    private static string ThrowIfInvalidName(string name)
    {
        const string ErrorMessage = "Name must not be empty or contain colons.";

        if (string.IsNullOrWhiteSpace(name) || name.Contains(':'))
        {
            throw new ArgumentException(ErrorMessage);
        }

        return name;
    }

    private static ImmutableList<string> ThrowIfInvalidPermissions(ImmutableList<string> permissions)
    {
        const string ErrorMessage = "Permissions must contain at least one permission and each permission must not be empty.";

        if (permissions.Count == 0 || permissions.Any(string.IsNullOrWhiteSpace))
        {
            throw new ArgumentException(ErrorMessage);
        }

        return permissions;
    }
}
