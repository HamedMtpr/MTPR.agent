using MTPR.Agent.Core.Abstractions;
using MTPR.Agent.Core.Models;

namespace MTPR.Agent.Core.Tools;

/// <summary>
/// Creates or overwrites a file inside the workspace after the user approves.
/// </summary>
public sealed class WriteFileTool(IPathPolicy pathPolicy, IApprovalService approvalService) : ITool
{
    /// <inheritdoc />
    public ToolDefinition Definition { get; } = new(
        "write_file",
        "Creates or overwrites a file in the workspace. Requires user approval.",
        ToolJson.Schema(
            """
            {
              "type": "object",
              "properties": {
                "path": { "type": "string" },
                "content": { "type": "string" }
              },
              "required": ["path", "content"],
              "additionalProperties": false
            }
            """));

    /// <inheritdoc />
    public async Task<ToolResult> ExecuteAsync(ToolCall call, CancellationToken cancellationToken)
    {
        var requested = ToolJson.GetString(call.Arguments, "path");
        var content = ToolJson.GetString(call.Arguments, "content");
        var path = pathPolicy.Resolve(requested);

        var approved = await approvalService.RequestApprovalAsync(
            Definition.Name,
            $"Write {content.Length} characters to '{requested}'.",
            cancellationToken);

        if (!approved)
        {
            return new(call.Id, ToolStatus.Denied, $"The user denied writing '{requested}'.");
        }

        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        await File.WriteAllTextAsync(path, content, cancellationToken);

        return new(call.Id, ToolStatus.Succeeded, $"Wrote {content.Length} characters to '{requested}'.");
    }
}
