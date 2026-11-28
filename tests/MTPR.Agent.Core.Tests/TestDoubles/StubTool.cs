using System.Text.Json;
using MTPR.Agent.Core.Abstractions;
using MTPR.Agent.Core.Models;
using Mtpr.TestUtilities;

namespace MTPR.Agent.Core.Tests.TestDoubles;

public sealed class StubTool(string name, string schema = "{}") : ITool
{
    public ToolDefinition Definition { get; } = new(
        name,
        $"Stub tool {name}",
        JsonTestData.Parse(schema));

    public int ExecutionCount { get; private set; }

    public Task<ToolResult> ExecuteAsync(ToolCall call, CancellationToken cancellationToken)
    {
        ExecutionCount++;

        return Task.FromResult(new ToolResult(call.Id, ToolStatus.Succeeded, Definition.Name));
    }
}
