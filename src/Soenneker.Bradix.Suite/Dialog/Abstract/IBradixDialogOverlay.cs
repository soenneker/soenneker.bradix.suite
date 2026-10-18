namespace Soenneker.Bradix;

/// <summary>
/// Describes the public API of <see cref="BradixDialogOverlay"/>.
/// </summary>
public interface IBradixDialogOverlay : System.IAsyncDisposable
{
    /// <summary>
    /// Gets or sets whether the overlay stays mounted while closed.
    /// </summary>
    bool ForceMount { get; set; }
}
