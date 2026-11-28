using MTPR.Agent.Core.Models;
using MTPR.Agent.Core.Tests.TestDoubles;
using MTPR.Agent.Core.Tools;

namespace MTPR.Agent.Core.Tests.Tools;

public sealed class WriteFileToolTests : WorkspaceTestBase
{
    private const string Arguments =
        """
        {
          "path": "sub/b.txt",
          "content": "data"
        }
        """;

    [Fact]
    public async Task ExecuteAsync_Should_WriteFile_When_Approved()
    {
        var approval = new StubApprovalService(approved: true);
        var tool = new WriteFileTool(PathPolicy, approval);

        var actual = await tool.ExecuteAsync(CreateCall("write_file", Arguments), TestContext.Current.CancellationToken);

        actual.Status.Should().Be(ToolStatus.Succeeded);
        (await File.ReadAllTextAsync(PathOf("sub/b.txt"), TestContext.Current.CancellationToken)).Should().Be("data");
        approval.RequestCount.Should().Be(1);
    }

    [Fact]
    public async Task ExecuteAsync_Should_NotWrite_When_Denied()
    {
        var tool = new WriteFileTool(PathPolicy, new StubApprovalService(approved: false));

        var actual = await tool.ExecuteAsync(CreateCall("write_file", Arguments), TestContext.Current.CancellationToken);

        actual.Status.Should().Be(ToolStatus.Denied);
        File.Exists(PathOf("sub/b.txt")).Should().BeFalse();
    }
}
