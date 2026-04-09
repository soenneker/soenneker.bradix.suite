using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Soenneker.Bradix.Suite.Abstract;

namespace Soenneker.Bradix.Suite;

/// <inheritdoc cref="IBradixComponent"/>
public sealed class BradixComponent : IBradixComponent
{
    private readonly ISuiteInterop _interop;

    public BradixComponent(ISuiteInterop interop)
    {
        _interop = interop ?? throw new ArgumentNullException(nameof(interop));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueTask Initialize(CancellationToken cancellationToken = default)
    {
        return _interop.Initialize(cancellationToken);
    }
}
