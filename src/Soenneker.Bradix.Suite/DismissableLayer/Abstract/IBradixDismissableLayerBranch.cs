using System.Threading.Tasks;

namespace Soenneker.Bradix;

/// <summary>
/// Describes the public API of <see cref="BradixDismissableLayerBranch"/>.
/// </summary>
public interface IBradixDismissableLayerBranch
{
    /// <summary>
    /// Releases resources used by dismissable layer branch registration.
    /// </summary>
    ValueTask DisposeAsync();
}
