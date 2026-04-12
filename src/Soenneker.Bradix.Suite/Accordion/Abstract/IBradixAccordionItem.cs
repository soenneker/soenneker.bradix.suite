namespace Soenneker.Bradix;

/// <summary>
/// Describes the public API of <see cref="BradixAccordionItem"/>.
/// </summary>
public interface IBradixAccordionItem
{
    /// <summary>
    /// Gets or sets the unique value identifying this accordion item.
    /// </summary>
    string Value { get; set; }

    /// <summary>
    /// Gets or sets whether this item is disabled.
    /// </summary>
    bool Disabled { get; set; }
}
