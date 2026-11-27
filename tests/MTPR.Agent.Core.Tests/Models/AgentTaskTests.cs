using MTPR.Agent.Core.Models;
using Mtpr.TestUtilities;

namespace MTPR.Agent.Core.Tests.Models;

public sealed class AgentTaskTests : UnitTest
{
    [Fact]
    public void Description_Should_Return_ProvidedValue()
    {
        var actual = new AgentTask("Add a unit test");

        actual.Description.Should().Be("Add a unit test");
    }
}
