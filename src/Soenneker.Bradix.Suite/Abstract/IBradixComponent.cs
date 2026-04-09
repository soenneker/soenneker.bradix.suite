using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.Bradix.Suite.Abstract;

/// <summary>
/// A higher-level Blazor utility built on top of <see cref="ISuiteInterop"/>.
/// </summary>
public interface IBradixComponent
{
    /// <summary>
    /// Ensures the underlying JavaScript module has been loaded and is ready for use.
    /// </summary>
    ValueTask Initialize(CancellationToken cancellationToken = default);
}
