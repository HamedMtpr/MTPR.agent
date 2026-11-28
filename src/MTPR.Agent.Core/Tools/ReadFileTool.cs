using MTPR.Agent.Core.Abstractions;
using MTPR.Agent.Core.Models;

namespace MTPR.Agent.Core.Tools;

/// <summary>
/// Reads the text content of a file inside the workspace.
/// </summary>
public sealed class ReadFileTool(IPathPolicy pathPolicy) : ITool
{
    /// <inheritdoc />
    public ToolDefinition Definition { get; } = new(
        "read_file",
        "Reads the text content of a file in the workspace.",
        ToolJson.Schema(
            """
            {
              "type": "object",
              "properties": {
                "path": { "type": "string" }
              },
              "required": ["path"],
              "additionalProperties": false
            }
            """));

    /// <inheritdoc />
    public async Task<ToolResult> ExecuteAsync(ToolCall call, CancellationToken cancellationToken)
    {
        var path = pathPolicy.Resolve(ToolJson.GetString(call.Arguments, "path"));
        var content = await File.ReadAllTextAsync(path, cancellationToken);

        return new(call.Id, ToolStatus.Succeeded, content);
    }
}
