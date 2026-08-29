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
    /// Initializes the Bradix Suite so it is ready for use.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the Bradix Suite is ready for use.</returns>
    ValueTask Initialize(CancellationToken cancellationToken = default);
}
