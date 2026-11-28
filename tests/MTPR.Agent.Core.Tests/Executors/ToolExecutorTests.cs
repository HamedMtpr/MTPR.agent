using MTPR.Agent.Core.Abstractions;
using MTPR.Agent.Core.Executors;
using MTPR.Agent.Core.Models;
using MTPR.Agent.Core.Registries;
using MTPR.Agent.Core.Tests.TestDoubles;
using MTPR.Agent.Core.Validators;
using Mtpr.TestUtilities;

namespace MTPR.Agent.Core.Tests.Executors;

public sealed class ToolExecutorTests : UnitTest
{
    private const string PathSchema = """
        {
          "type": "object",
          "properties": {
            "path": { "type": "string" }
          },
          "required": ["path"]
        }
        """;

    [Fact]
    public async Task ExecuteAsync_Should_ReturnFailed_When_ToolIsUnknown()
    {
        var executor = createExecutor(new StubTool("file_read"));

        var actual = await executor.ExecuteAsync(createCall("missing", "{}"), CancellationToken.None);

        actual.Status.Should().Be(ToolStatus.Failed);
        actual.Content.Should().Be("Unknown tool 'missing'.");
    }

    [Fact]
    public async Task ExecuteAsync_Should_ReturnFailedWithoutInvoking_When_ArgumentsAreInvalid()
    {
        var tool = new StubTool("file_read", PathSchema);
        var executor = createExecutor(tool);

        var actual = await executor.ExecuteAsync(createCall("file_read", "{}"), CancellationToken.None);

        actual.Status.Should().Be(ToolStatus.Failed);
        actual.Content.Should().StartWith("Invalid arguments:").And.Contain("'path' is missing");
        tool.ExecutionCount.Should().Be(0);
    }

    [Fact]
    public async Task ExecuteAsync_Should_InvokeTool_When_ArgumentsAreValid()
    {
        var tool = new StubTool("file_read", PathSchema);
        var executor = createExecutor(tool);

        var actual = await executor.ExecuteAsync(
            createCall("file_read", """{ "path": "a.txt" }"""),
            CancellationToken.None);

        actual.CallId.Should().Be("call-1");
        actual.Status.Should().Be(ToolStatus.Succeeded);
        tool.ExecutionCount.Should().Be(1);
    }

    [Fact]
    public async Task ExecuteAsync_Should_ReturnFailed_When_ToolThrows()
    {
        var executor = createExecutor(new ThrowingTool(new IOException("disk offline")));

        var actual = await executor.ExecuteAsync(createCall("throwing_tool", "{}"), CancellationToken.None);

        actual.Status.Should().Be(ToolStatus.Failed);
        actual.Content.Should().Be("Tool 'throwing_tool' failed: disk offline");
    }

    [Fact]
    public async Task ExecuteAsync_Should_Rethrow_When_CancellationIsRequested()
    {
        using var cancellation = new CancellationTokenSource();
        await cancellation.CancelAsync();
        var executor = createExecutor(new ThrowingTool(new OperationCanceledException()));

        var act = () => executor.ExecuteAsync(createCall("throwing_tool", "{}"), cancellation.Token);

        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task ExecuteAsync_Should_ThrowArgumentNullException_When_CallIsNull()
    {
        var executor = createExecutor(new StubTool("file_read"));

        var act = () => executor.ExecuteAsync(null!, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    private static ToolExecutor createExecutor(params ITool[] tools) =>
        new(new ToolRegistry(tools), new JsonSchemaValidator());

    private static ToolCall createCall(string name, string arguments) =>
        new("call-1", name, JsonTestData.Parse(arguments));
}
