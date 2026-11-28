using MTPR.Agent.Core.Registries;
using MTPR.Agent.Core.Tests.TestDoubles;
using Mtpr.TestUtilities;

namespace MTPR.Agent.Core.Tests.Registries;

public sealed class ToolRegistryTests : UnitTest
{
    [Fact]
    public void Find_Should_ReturnRegisteredTool()
    {
        var tool = new StubTool("file_read");
        var registry = new ToolRegistry([tool]);

        var actual = registry.Find("file_read");

        actual.Should().BeSameAs(tool);
    }

    [Fact]
    public void Find_Should_ReturnNull_WhenNameIsUnknown()
    {
        var registry = new ToolRegistry([new StubTool("file_read")]);

        var actual = registry.Find("file_write");

        actual.Should().BeNull();
    }

    [Fact]
    public void Find_Should_BeCaseSensitive()
    {
        var registry = new ToolRegistry([new StubTool("file_read")]);

        var actual = registry.Find("FILE_READ");

        actual.Should().BeNull();
    }

    [Fact]
    public void Constructor_Should_Throw_WhenNamesAreDuplicated()
    {
        var act = () => new ToolRegistry([new StubTool("file_read"), new StubTool("file_read")]);

        act.Should().Throw<ArgumentException>().WithMessage("*file_read*");
    }

    [Fact]
    public void Definitions_Should_ContainDefinitionOfEachTool()
    {
        var registry = new ToolRegistry([new StubTool("file_read"), new StubTool("git_status")]);

        var actual = registry.Definitions.Select(definition => definition.Name);

        actual.Should().BeEquivalentTo(["file_read", "git_status"]);
    }
}
