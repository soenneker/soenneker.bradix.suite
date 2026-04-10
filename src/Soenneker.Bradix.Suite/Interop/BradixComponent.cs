using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.Bradix;

public sealed class BradixComponent : IBradixComponent
{
    private readonly ISuiteInterop _suiteInterop;

    public BradixComponent(ISuiteInterop suiteInterop)
    {
        _suiteInterop = suiteInterop;
    }

    public ValueTask Initialize(CancellationToken cancellationToken = default)
    {
        return _suiteInterop.Initialize(cancellationToken);
    }
}
