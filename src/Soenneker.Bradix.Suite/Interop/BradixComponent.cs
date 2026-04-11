using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.Bradix;

///<inheritdoc cref="IBradixComponent"/>
public sealed class BradixComponent : IBradixComponent
{
    private readonly IBradixSuiteInterop _suiteInterop;

    public BradixComponent(IBradixSuiteInterop suiteInterop)
    {
        _suiteInterop = suiteInterop;
    }

    public ValueTask Initialize(CancellationToken cancellationToken = default)
    {
        return _suiteInterop.Initialize(cancellationToken);
    }
}
