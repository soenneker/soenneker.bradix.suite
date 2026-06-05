using System;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.Bradix;

/// <summary>
/// Optional all-in-one loader for Bradix JavaScript modules. Components should inject their focused interop directly.
/// </summary>
public interface IBradixSuiteInterop : IAsyncDisposable
{
    /// <summary>
    /// Executes the initialize operation.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask Initialize(CancellationToken cancellationToken = default);
}
