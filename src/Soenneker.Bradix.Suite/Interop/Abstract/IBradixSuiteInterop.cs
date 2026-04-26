using System;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.Bradix;

/// <summary>
/// Optional all-in-one loader for Bradix JavaScript modules. Components should inject their focused interop directly.
/// </summary>
public interface IBradixSuiteInterop : IAsyncDisposable
{
    ValueTask Initialize(CancellationToken cancellationToken = default);
}
