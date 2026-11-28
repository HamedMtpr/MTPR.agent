using System.Text.Json;
using MTPR.Agent.Core.Models;
using Mtpr.TestUtilities;

namespace MTPR.Agent.Core.Tests.Models;

public sealed class ToolCallTests : UnitTest
{
    [Fact]
    public void Constructor_Should_ExposeIdNameAndArguments()
    {
        var arguments = JsonDocument.Parse("{\"path\":\"a.txt\"}").RootElement;

        var actual = new ToolCall("call-1", "file_read", arguments);

        actual.Id.Should().Be("call-1");
        actual.Name.Should().Be("file_read");
        actual.Arguments.GetProperty("path").GetString().Should().Be("a.txt");
    }
}
