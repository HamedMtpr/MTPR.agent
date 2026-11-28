using MTPR.Agent.Core.Policies;
using Mtpr.TestUtilities;

namespace MTPR.Agent.Core.Tests.Policies;

public sealed class CommandAllowListPolicyTests : UnitTest
{
    private readonly CommandAllowListPolicy _policy = new(["git", "dotnet"]);

    [Fact]
    public void Constructor_Should_Throw_When_AllowListIsNull()
    {
        var act = () => new CommandAllowListPolicy(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void IsAllowed_Should_Throw_When_CommandIsNull()
    {
        var act = () => _policy.IsAllowed(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [InlineData("git status")]
    [InlineData("  dotnet   build -c Release")]
    [InlineData("GIT diff")]
    [InlineData("git")]
    public void IsAllowed_Should_ReturnTrue_When_ExecutableIsListed(string commandLine)
    {
        _policy.IsAllowed(commandLine).Should().BeTrue();
    }

    [Theory]
    [InlineData("rm -rf /")]
    [InlineData("./git status")]
    [InlineData("")]
    [InlineData("   ")]
    public void IsAllowed_Should_ReturnFalse_When_ExecutableIsNotListed(string commandLine)
    {
        _policy.IsAllowed(commandLine).Should().BeFalse();
    }

    [Theory]
    [InlineData("git status; rm -rf /")]
    [InlineData("git status && rm file")]
    [InlineData("git log | sh")]
    [InlineData("git log > out.txt")]
    [InlineData("git log `whoami`")]
    [InlineData("git log $(whoami)")]
    [InlineData("git status\nrm file")]
    public void IsAllowed_Should_ReturnFalse_When_CommandChainsOtherCommands(string commandLine)
    {
        _policy.IsAllowed(commandLine).Should().BeFalse();
    }
}
