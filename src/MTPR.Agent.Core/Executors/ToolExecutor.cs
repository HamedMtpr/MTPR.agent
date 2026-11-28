using MTPR.Agent.Core.Abstractions;
using MTPR.Agent.Core.Models;

namespace MTPR.Agent.Core.Executors;

/// <summary>
/// Resolves tools from the registry, validates their arguments, and invokes them.
/// </summary>
public sealed class ToolExecutor(IToolRegistry registry, IJsonSchemaValidator validator) : IToolExecutor
{
    /// <inheritdoc />
    public async Task<ToolResult> ExecuteAsync(ToolCall call, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(call);

        var tool = registry.Find(call.Name);

        if (tool is null)
        {
            return failed(call, $"Unknown tool '{call.Name}'.");
        }

        var validation = validator.Validate(tool.Definition.ParametersSchema, call.Arguments);

        if (!validation.IsValid)
        {
            return failed(call, $"Invalid arguments: {string.Join(" ", validation.Errors)}");
        }

        try
        {
            return await tool.ExecuteAsync(call, cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            return failed(call, $"Tool '{call.Name}' failed: {exception.Message}");
        }
    }

    private static ToolResult failed(ToolCall call, string content) =>
        new(call.Id, ToolStatus.Failed, content);
}
