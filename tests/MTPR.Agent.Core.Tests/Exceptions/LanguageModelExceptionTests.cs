using MTPR.Agent.Core.Exceptions;
using Mtpr.TestUtilities;

namespace MTPR.Agent.Core.Tests.Exceptions;

public sealed class LanguageModelExceptionTests : UnitTest
{
    [Fact]
    public void Constructor_Should_LeaveStatusCodeEmpty_When_OnlyMessageIsGiven()
    {
        var actual = new LanguageModelException("Failed");

        actual.Message.Should().Be("Failed");
        actual.StatusCode.Should().BeNull();
    }

    [Fact]
    public void Constructor_Should_KeepInnerException()
    {
        var inner = new InvalidOperationException();

        var actual = new LanguageModelException("Failed", inner);

        actual.InnerException.Should().BeSameAs(inner);
    }

    [Fact]
    public void Constructor_Should_ExposeStatusCode()
    {
        var actual = new LanguageModelException("Failed", 429);

        actual.StatusCode.Should().Be(429);
    }
}
