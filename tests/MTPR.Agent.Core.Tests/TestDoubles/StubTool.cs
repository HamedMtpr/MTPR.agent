using MTPR.Agent.Core.Abstractions;
using MTPR.Agent.Core.Models;

namespace MTPR.Agent.Core.Tests.TestDoubles;

public sealed class StubTool(string name) : ITool
{
    public ToolDefinition Definition { get; } = new(
        name,
        $"Stub tool {name}",
        System.Text.Json.JsonDocument.Parse("{}").RootElement);

    public Task<ToolResult> ExecuteAsync(ToolCall call, CancellationToken cancellationToken) =>
        Task.FromResult(new ToolResult(call.Id, ToolStatus.Succeeded, Definition.Name));
}
