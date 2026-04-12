using System.Threading.Tasks;

namespace Soenneker.Bradix;

/// <summary>
/// Describes the public API of <see cref="BradixAvatarFallback"/>.
/// </summary>
public interface IBradixAvatarFallback
{
    /// <summary>
    /// Gets or sets the delay in milliseconds before the fallback content is shown.
    /// </summary>
    int? DelayMs { get; set; }

    /// <summary>
    /// Releases resources used by the fallback delay timer.
    /// </summary>
    ValueTask DisposeAsync();
}
