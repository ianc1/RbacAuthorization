namespace RbacAuthorization.Internal;

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using RbacAuthorization.Utilities;

internal sealed class PathScope : ValueObject
{
    private static readonly Regex PathSegmentRegex = new Regex(@"^(/[^/{}\s]+)+$", RegexOptions.Compiled);

    public PathScope(string pathScope)
    {
        ArgumentNullException.ThrowIfNull(pathScope);

        Value = ThrowIfInvalidPathScope(pathScope);
    }

    public string Value { get; }

    protected override IEnumerable<IComparable?> GetEqualityComponents()
    {
        yield return Value;
    }

    private static string ThrowIfInvalidPathScope(string pathScope)
    {
        const string ErrorMessage = "PathScope must be a valid absolute URL path and not end with a slash.";

        if (!PathSegmentRegex.Match(pathScope).Success)
        {
            throw new ArgumentException(ErrorMessage);
        }

        return pathScope;
    }
}
