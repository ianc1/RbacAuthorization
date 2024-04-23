namespace RbacAuthorization.Internal;

using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using RbacAuthorization.Utilities;

internal sealed class PathScopeDefinition : ValueObject
{
    private static readonly Regex PathSegmentRegex = new Regex(@"^(/{?[^/{}\s]+}?)+$", RegexOptions.Compiled);

    private static readonly Regex PathSegmentPlaceholderRegex = new Regex(@"\{(\S+?)\}", RegexOptions.Compiled);

    private readonly Regex? PathScopeParametersRegex;

    private readonly ImmutableList<string> PathScopeParameterPlaceholders;

    public PathScopeDefinition(string pathScope)
    {
        ArgumentNullException.ThrowIfNull(pathScope);

        Value = ThrowIfInvalidPathScope(pathScope);

        PathScopeParametersRegex = ConvertPathScopeToRegex(pathScope);

        PathScopeParameterPlaceholders = ConvertRegexCaptureGroupsToPathScopePlaceholders(PathScopeParametersRegex);
    }

    public string Value { get; }

    public PathScope ToPathScope(params string[] pathScopeParameters)
    {
        if (PathScopeParameterPlaceholders.Count != pathScopeParameters.Length)
        {
            throw new ArgumentException($"PathScope definition requires {PathScopeParameterPlaceholders.Count} parameters but {pathScopeParameters.Length} were supplied.");
        }

        if (pathScopeParameters.Any(string.IsNullOrWhiteSpace))
        {
            throw new ArgumentException($"PathScope definition parameters must not be empty.");
        }

        if (PathScopeParameterPlaceholders.Count == 0)
        {
            return new PathScope(Value);
        }

        var scope = new StringBuilder(Value);

        for (var i = 0; i < PathScopeParameterPlaceholders.Count; i++)
        {
            scope.Replace(PathScopeParameterPlaceholders[i], pathScopeParameters[i]);
        }

        return new PathScope(scope.ToString());
    }

    public bool MatchesRequestPath(PathString requestPath, [NotNullWhen(true)] out PathScope? requiredPathScope)
    {
        ArgumentNullException.ThrowIfNull(requestPath);

        if (PathScopeParameterPlaceholders.Count == 0)
        {
            if (!requestPath.StartsWithSegments(Value))
            {
                requiredPathScope = null;
                return false;
            }

            requiredPathScope = new PathScope(Value);
            return true;
        }

        var match = PathScopeParametersRegex!.Match(requestPath);

        if (!match.Success)
        {
            requiredPathScope = null;
            return false;
        }

        requiredPathScope = new PathScope(match.Groups[0].Value);
        return true;
    }

    protected override IEnumerable<IComparable?> GetEqualityComponents()
    {
        yield return Value;
    }

    private static string ThrowIfInvalidPathScope(string pathScope)
    {
        const string ErrorMessage = "PathScope must be a valid absolute URL path with optional parameter segments and not end with a slash.";

        if (!PathSegmentRegex.Match(pathScope).Success)
        {
            throw new ArgumentException(ErrorMessage);
        }

        return pathScope;
    }

    private static Regex ConvertPathScopeToRegex(string scope)
    {
        var pattern = PathSegmentPlaceholderRegex.Replace(scope, match =>
        {
            var parameterName = match.Groups[1].Value;

            return $"(?<{parameterName}>[^/]+)";
        });

        return new Regex($"^{pattern}", RegexOptions.Compiled);
    }

    private static ImmutableList<string> ConvertRegexCaptureGroupsToPathScopePlaceholders(Regex regex)
    {
        return regex.GetGroupNames()
            .Where(name => name != "0")
            .Select(name => $"{{{name}}}")
            .ToImmutableList();
    }
}
