namespace Soenneker.Bradix;

/// <summary>
/// Describes the public API of <see cref="BradixDialogOverlay"/>.
/// </summary>
public interface IBradixDialogOverlay
{
    /// <summary>
    /// Gets or sets whether the overlay stays mounted while closed.
    /// </summary>
    bool ForceMount { get; set; }
}
