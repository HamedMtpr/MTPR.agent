using MTPR.Agent.Core.Models;
using MTPR.Agent.Core.Tools;

namespace MTPR.Agent.Core.Tests.Tools;

public sealed class SearchFilesToolTests : WorkspaceTestBase
{
    [Fact]
    public async Task ExecuteAsync_Should_ReturnMatchingLines_IgnoringCase()
    {
        Directory.CreateDirectory(PathOf("src"));
        await File.WriteAllTextAsync(PathOf("src/a.txt"), "one\n  Needle here\nthree", TestContext.Current.CancellationToken);
        var tool = new SearchFilesTool(PathPolicy);

        var actual = await tool.ExecuteAsync(CreateCall("search_files", """{ "query": "needle" }"""), TestContext.Current.CancellationToken);

        actual.Status.Should().Be(ToolStatus.Succeeded);
        actual.Content.Should().Be("src/a.txt:2: Needle here");
    }

    [Fact]
    public async Task ExecuteAsync_Should_ReturnNoMatches_When_NothingFound()
    {
        await File.WriteAllTextAsync(PathOf("a.txt"), "abc", TestContext.Current.CancellationToken);
        var tool = new SearchFilesTool(PathPolicy);

        var actual = await tool.ExecuteAsync(CreateCall("search_files", """{ "query": "zzz" }"""), TestContext.Current.CancellationToken);

        actual.Content.Should().Be("No matches.");
    }

    [Fact]
    public async Task ExecuteAsync_Should_SkipGitDirectoryAndBinaryFiles()
    {
        Directory.CreateDirectory(PathOf(".git"));
        await File.WriteAllTextAsync(PathOf(".git/config"), "needle", TestContext.Current.CancellationToken);
        await File.WriteAllTextAsync(PathOf("bin.dat"), "needle\0", TestContext.Current.CancellationToken);
        var tool = new SearchFilesTool(PathPolicy);

        var actual = await tool.ExecuteAsync(CreateCall("search_files", """{ "query": "needle" }"""), TestContext.Current.CancellationToken);

        actual.Content.Should().Be("No matches.");
    }

    [Fact]
    public async Task ExecuteAsync_Should_TruncateResults_When_LimitIsExceeded()
    {
        await File.WriteAllLinesAsync(PathOf("a.txt"), Enumerable.Repeat("needle", 101), TestContext.Current.CancellationToken);
        var tool = new SearchFilesTool(PathPolicy);

        var actual = await tool.ExecuteAsync(CreateCall("search_files", """{ "query": "needle" }"""), TestContext.Current.CancellationToken);

        actual.Content.Should().EndWith("(results truncated)");
        actual.Content.Split('\n').Should().HaveCount(101);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_When_DirectoryIsMissing()
    {
        var tool = new SearchFilesTool(PathPolicy);

        var act = () => tool.ExecuteAsync(CreateCall("search_files", """{ "query": "x", "path": "none" }"""), TestContext.Current.CancellationToken);

        await act.Should().ThrowAsync<DirectoryNotFoundException>();
    }
}
