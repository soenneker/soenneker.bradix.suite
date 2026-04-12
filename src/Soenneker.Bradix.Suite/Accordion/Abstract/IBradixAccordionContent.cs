namespace Soenneker.Bradix;

/// <summary>
/// Describes the public API of <see cref="BradixAccordionContent"/>.
/// </summary>
public interface IBradixAccordionContent
{
    /// <summary>
    /// Gets or sets whether the content stays mounted in the DOM while closed.
    /// </summary>
    bool ForceMount { get; set; }
}
