using System.Text.Json;
using MTPR.Agent.Core.Models;
using MTPR.Agent.Core.Policies;
using Mtpr.TestUtilities;

namespace MTPR.Agent.Core.Tests.Tools;

public abstract class WorkspaceTestBase : UnitTest, IDisposable
{
    protected WorkspaceTestBase()
    {
        Root = Path.Combine(Path.GetTempPath(), "mtpr-tools-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(Root);
        PathPolicy = new(Root);
    }

    protected string Root { get; }

    protected WorkspacePathPolicy PathPolicy { get; }

    public void Dispose()
    {
        Directory.Delete(Root, recursive: true);
        GC.SuppressFinalize(this);
    }

    protected static ToolCall CreateCall(string name, string argumentsJson) =>
        new("call-1", name, JsonDocument.Parse(argumentsJson).RootElement);

    protected string PathOf(string relative) => Path.Combine(Root, relative);
}
