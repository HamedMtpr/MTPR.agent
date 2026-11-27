using MTPR.Agent.Core.Models;
using Mtpr.TestUtilities;

namespace MTPR.Agent.Core.Tests.Models;

public sealed class TokenUsageTests : UnitTest
{
    [Fact]
    public void TotalTokens_Should_SumPromptAndCompletionTokens()
    {
        var actual = new TokenUsage(5, 2);

        actual.TotalTokens.Should().Be(7);
    }
}
