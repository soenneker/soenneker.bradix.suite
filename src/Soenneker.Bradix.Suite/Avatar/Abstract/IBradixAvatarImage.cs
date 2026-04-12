using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Describes the public API of <see cref="BradixAvatarImage"/>.
/// </summary>
public interface IBradixAvatarImage
{
    /// <summary>
    /// Gets or sets the image URL.
    /// </summary>
    string? Src { get; set; }

    /// <summary>
    /// Gets or sets the alternative text for the image.
    /// </summary>
    string? Alt { get; set; }

    /// <summary>
    /// Gets or sets the CORS mode for the image request.
    /// </summary>
    string? CrossOrigin { get; set; }

    /// <summary>
    /// Gets or sets the referrer policy for the image request.
    /// </summary>
    string? ReferrerPolicy { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when the loading status changes.
    /// </summary>
    EventCallback<BradixAvatarImageLoadingStatus> OnLoadingStatusChange { get; set; }

    /// <summary>
    /// Releases resources used by the image loading bridge.
    /// </summary>
    ValueTask DisposeAsync();

    /// <summary>
    /// Handles a loading status update emitted from JavaScript.
    /// </summary>
    /// <param name="status">The serialized loading status value.</param>
    Task HandleImageLoadingStatusChanged(string status);
}
