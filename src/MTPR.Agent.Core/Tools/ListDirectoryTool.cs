using MTPR.Agent.Core.Abstractions;
using MTPR.Agent.Core.Models;

namespace MTPR.Agent.Core.Tools;

/// <summary>
/// Lists the files and directories directly inside a workspace directory.
/// </summary>
public sealed class ListDirectoryTool(IPathPolicy pathPolicy) : ITool
{
    /// <inheritdoc />
    public ToolDefinition Definition { get; } = new(
        "list_directory",
        "Lists files and directories in a workspace directory. Directories end with '/'.",
        ToolJson.Schema(
            """
            {
              "type": "object",
              "properties": {
                "path": { "type": "string" }
              },
              "additionalProperties": false
            }
            """));

    /// <inheritdoc />
    public Task<ToolResult> ExecuteAsync(ToolCall call, CancellationToken cancellationToken)
    {
        var path = pathPolicy.Resolve(ToolJson.GetOptionalString(call.Arguments, "path", "."));

        if (!Directory.Exists(path))
        {
            throw new DirectoryNotFoundException($"Directory not found: {path}");
        }

        var entries = Directory.
            EnumerateFileSystemEntries(path).
            Select(entry => Directory.Exists(entry) ? Path.GetFileName(entry) + "/" : Path.GetFileName(entry)).
            Order(StringComparer.Ordinal).
            ToList();

        var content = entries.Count == 0 ? "(empty)" : string.Join('\n', entries);

        return Task.FromResult(new ToolResult(call.Id, ToolStatus.Succeeded, content));
    }
}
