using MTPR.Agent.Core.Abstractions;

namespace MTPR.Agent.Core.Tests.TestDoubles;

public sealed class StubApprovalService(bool approved) : IApprovalService
{
    public int RequestCount { get; private set; }

    public Task<bool> RequestApprovalAsync(string action, string details, CancellationToken cancellationToken)
    {
        RequestCount++;

        return Task.FromResult(approved);
    }
}
