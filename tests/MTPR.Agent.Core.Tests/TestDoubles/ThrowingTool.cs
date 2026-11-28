using MTPR.Agent.Core.Abstractions;
using MTPR.Agent.Core.Models;
using Mtpr.TestUtilities;

namespace MTPR.Agent.Core.Tests.TestDoubles;

public sealed class ThrowingTool(Exception exception) : ITool
{
    public ToolDefinition Definition { get; } = new(
        "throwing_tool",
        "Tool that always throws",
        JsonTestData.Parse("{}"));

    public Task<ToolResult> ExecuteAsync(ToolCall call, CancellationToken cancellationToken) =>
        throw exception;
}
