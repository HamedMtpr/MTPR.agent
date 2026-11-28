using MTPR.Agent.Core.Models;
using MTPR.Agent.Core.Tools;

namespace MTPR.Agent.Core.Tests.Tools;

public sealed class ListDirectoryToolTests : WorkspaceTestBase
{
    [Fact]
    public async Task ExecuteAsync_Should_ListEntries_When_PathIsOmitted()
    {
        await File.WriteAllTextAsync(PathOf("b.txt"), "", TestContext.Current.CancellationToken);
        Directory.CreateDirectory(PathOf("a"));
        var tool = new ListDirectoryTool(PathPolicy);

        var actual = await tool.ExecuteAsync(CreateCall("list_directory", "{}"), TestContext.Current.CancellationToken);

        actual.Status.Should().Be(ToolStatus.Succeeded);
        actual.Content.Should().Be("a/\nb.txt");
    }

    [Fact]
    public async Task ExecuteAsync_Should_ReturnEmptyMarker_When_DirectoryIsEmpty()
    {
        Directory.CreateDirectory(PathOf("empty"));
        var tool = new ListDirectoryTool(PathPolicy);

        var actual = await tool.ExecuteAsync(CreateCall("list_directory", """{ "path": "empty" }"""), TestContext.Current.CancellationToken);

        actual.Content.Should().Be("(empty)");
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_When_DirectoryIsMissing()
    {
        var tool = new ListDirectoryTool(PathPolicy);

        var act = () => tool.ExecuteAsync(CreateCall("list_directory", """{ "path": "none" }"""), TestContext.Current.CancellationToken);

        await act.Should().ThrowAsync<DirectoryNotFoundException>();
    }

    [Fact]
    public void Definition_Should_BeNamedListDirectory()
    {
        new ListDirectoryTool(PathPolicy).Definition.Name.Should().Be("list_directory");
    }
}
