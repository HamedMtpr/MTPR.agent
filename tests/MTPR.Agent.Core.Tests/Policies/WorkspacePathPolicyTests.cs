using MTPR.Agent.Core.Exceptions;
using MTPR.Agent.Core.Policies;
using Mtpr.TestUtilities;

namespace MTPR.Agent.Core.Tests.Policies;

public sealed class WorkspacePathPolicyTests : UnitTest
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), "workspace");
    private readonly WorkspacePathPolicy _policy;

    public WorkspacePathPolicyTests()
    {
        _policy = new(_root);
    }

    [Fact]
    public void Constructor_Should_Throw_When_RootIsBlank()
    {
        var act = () => new WorkspacePathPolicy(" ");

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Resolve_Should_Throw_When_PathIsNull()
    {
        var act = () => _policy.Resolve(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [InlineData("file.txt")]
    [InlineData("src/app/file.txt")]
    [InlineData("src/../file.txt")]
    [InlineData("..file.txt")]
    public void Resolve_Should_ReturnAbsolutePath_When_PathIsInsideWorkspace(string path)
    {
        var actual = _policy.Resolve(path);

        actual.Should().StartWith(Path.GetFullPath(_root));
        Path.IsPathRooted(actual).Should().BeTrue();
    }

    [Fact]
    public void Resolve_Should_ReturnRoot_When_PathIsEmpty()
    {
        var actual = _policy.Resolve(string.Empty);

        actual.Should().Be(Path.GetFullPath(_root));
    }

    [Fact]
    public void Resolve_Should_AcceptAbsolutePath_When_InsideWorkspace()
    {
        var path = Path.Combine(_root, "a.txt");

        var actual = _policy.Resolve(path);

        actual.Should().Be(Path.GetFullPath(path));
    }

    [Theory]
    [InlineData("..")]
    [InlineData("../other.txt")]
    [InlineData("src/../../other.txt")]
    public void Resolve_Should_Throw_When_PathEscapesWorkspace(string path)
    {
        var act = () => _policy.Resolve(path);

        act.Should().
            Throw<PolicyViolationException>().
            WithMessage($"*'{path}'*");
    }

    [Fact]
    public void Resolve_Should_Throw_When_AbsolutePathIsOutsideWorkspace()
    {
        var path = Path.Combine(Path.GetTempPath(), "workspace-other", "a.txt");

        var act = () => _policy.Resolve(path);

        act.Should().Throw<PolicyViolationException>();
    }
}
