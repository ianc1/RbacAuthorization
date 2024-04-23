namespace RbacAuthorization;

using System;
using System.Collections.Generic;
using RbacAuthorization.Internal;
using RbacAuthorization.Utilities;

public class Role : ValueObject
{
    private readonly PathScope? pathScope;

    public Role(string name, string? pathScope = null)
    {
        ArgumentNullException.ThrowIfNull(name);

        Name = ThrowIfInvalidName(name);
        this.pathScope = pathScope is null ? null : new PathScope(pathScope);
    }

    public string Name { get; }

    public string? PathScope => pathScope?.Value;

    public static Role Parse(string role)
    {
        ArgumentNullException.ThrowIfNull(role);

        var components = role.Split(':', 2);

        return new Role(
            name: components[0],
            pathScope: components.Length == 2 ? components[1] : null);
    }
    public override string ToString()
    {
        return pathScope is null ? Name : $"{Name}:{PathScope}";
    }

    protected override IEnumerable<IComparable?> GetEqualityComponents()
    {
        yield return Name;
        yield return pathScope;
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
}
