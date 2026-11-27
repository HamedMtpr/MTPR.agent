using MTPR.Agent.Core.Models;
using Mtpr.TestUtilities;

namespace MTPR.Agent.Core.Tests.Models;

public sealed class AgentResultTests : UnitTest
{
    [Fact]
    public void Constructor_ShouldExposeStatusAndSummary()
    {
        var actual = new AgentResult(AgentStatus.Succeeded, "Done");

        actual.Status.Should().Be(AgentStatus.Succeeded);
        actual.Summary.Should().Be("Done");
    }
}
