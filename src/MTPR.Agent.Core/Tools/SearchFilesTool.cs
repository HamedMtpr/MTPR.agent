using MTPR.Agent.Core.Abstractions;
using MTPR.Agent.Core.Models;

namespace MTPR.Agent.Core.Tools;

/// <summary>
/// Searches the text files under a workspace directory for lines containing a query.
/// </summary>
public sealed class SearchFilesTool(IPathPolicy pathPolicy) : ITool
{
    private const int MaxMatches = 100;
    private const long MaxFileBytes = 1_000_000;
    private const string GitDirectory = ".git";

    /// <inheritdoc />
    public ToolDefinition Definition { get; } = new(
        "search_files",
        "Searches text files for lines containing the query (case-insensitive). Returns 'path:line: text'.",
        ToolJson.Schema(
            """
            {
              "type": "object",
              "properties": {
                "query": { "type": "string" },
                "path": { "type": "string" }
              },
              "required": ["query"],
              "additionalProperties": false
            }
            """));

    /// <inheritdoc />
    public async Task<ToolResult> ExecuteAsync(ToolCall call, CancellationToken cancellationToken)
    {
        var query = ToolJson.GetString(call.Arguments, "query");
        var root = pathPolicy.Resolve(ToolJson.GetOptionalString(call.Arguments, "path", "."));

        if (!Directory.Exists(root))
        {
            throw new DirectoryNotFoundException($"Directory not found: {root}");
        }

        List<string> matches = [];

        foreach (var file in enumerateFiles(root))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!await collectMatchesAsync(root, file, query, matches, cancellationToken))
            {
                matches.Add("(results truncated)");
                break;
            }
        }

        var content = matches.Count == 0 ? "No matches." : string.Join('\n', matches);

        return new(call.Id, ToolStatus.Succeeded, content);
    }

    private static IEnumerable<string> enumerateFiles(string root) =>
        Directory.
            EnumerateFiles(root, "*", new EnumerationOptions { RecurseSubdirectories = true, IgnoreInaccessible = true }).
            Where(file => !isInsideGitDirectory(root, file)).
            Where(file => new FileInfo(file).Length <= MaxFileBytes).
            Order(StringComparer.Ordinal);

    private static bool isInsideGitDirectory(string root, string file) =>
        Path.GetRelativePath(root, file).
            Split(Path.DirectorySeparatorChar).
            Contains(GitDirectory);

    /// <returns><see langword="false"/> when the match limit was reached.</returns>
    private static async Task<bool> collectMatchesAsync(
        string root,
        string file,
        string query,
        List<string> matches,
        CancellationToken cancellationToken)
    {
        var lines = await File.ReadAllLinesAsync(file, cancellationToken);

        if (lines.Any(line => line.Contains('\0')))
        {
            return true;
        }

        var name = Path.GetRelativePath(root, file).Replace(Path.DirectorySeparatorChar, '/');

        for (var index = 0; index < lines.Length; index++)
        {
            if (!lines[index].Contains(query, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (matches.Count >= MaxMatches)
            {
                return false;
            }

            matches.Add($"{name}:{index + 1}: {lines[index].Trim()}");
        }

        return true;
    }
}
