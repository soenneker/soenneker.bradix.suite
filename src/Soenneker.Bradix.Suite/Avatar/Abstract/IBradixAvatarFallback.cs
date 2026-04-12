using System;

namespace Soenneker.Bradix;

/// <summary>
/// Describes the public API of <see cref="BradixAvatarFallback"/>.
/// </summary>
public interface IBradixAvatarFallback : IAsyncDisposable
{
    /// <summary>
    /// Gets or sets the delay in milliseconds before the fallback content is shown.
    /// </summary>
    int? DelayMs { get; set; }
}