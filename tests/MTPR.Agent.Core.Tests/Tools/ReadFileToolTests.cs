using MTPR.Agent.Core.Exceptions;
using MTPR.Agent.Core.Models;
using MTPR.Agent.Core.Tools;

namespace MTPR.Agent.Core.Tests.Tools;

public sealed class ReadFileToolTests : WorkspaceTestBase
{
    [Fact]
    public async Task ExecuteAsync_Should_ReturnContent_When_FileExists()
    {
        await File.WriteAllTextAsync(PathOf("a.txt"), "hello", TestContext.Current.CancellationToken);
        var tool = new ReadFileTool(PathPolicy);

        var actual = await tool.ExecuteAsync(CreateCall("read_file", """{ "path": "a.txt" }"""), TestContext.Current.CancellationToken);

        actual.Status.Should().Be(ToolStatus.Succeeded);
        actual.Content.Should().Be("hello");
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_When_PathEscapesWorkspace()
    {
        var tool = new ReadFileTool(PathPolicy);

        var act = () => tool.ExecuteAsync(CreateCall("read_file", """{ "path": "../x" }"""), TestContext.Current.CancellationToken);

        await act.Should().ThrowAsync<PolicyViolationException>();
    }

    [Fact]
    public void Definition_Should_BeNamedReadFile()
    {
        new ReadFileTool(PathPolicy).Definition.Name.Should().Be("read_file");
    }
}
