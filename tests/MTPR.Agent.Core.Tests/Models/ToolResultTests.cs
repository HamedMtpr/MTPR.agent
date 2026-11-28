using MTPR.Agent.Core.Models;
using Mtpr.TestUtilities;

namespace MTPR.Agent.Core.Tests.Models;

public sealed class ToolResultTests : UnitTest
{
    [Fact]
    public void Constructor_ShouldExposeCallIdStatusAndContent()
    {
        var actual = new ToolResult("call-1", ToolStatus.Denied, "Path is outside the workspace");

        actual.CallId.Should().Be("call-1");
        actual.Status.Should().Be(ToolStatus.Denied);
        actual.Content.Should().Be("Path is outside the workspace");
    }
}
