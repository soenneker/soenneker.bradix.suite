using System.Threading.Tasks;

namespace Soenneker.Bradix;

/// <summary>
/// Internal contract for menu items participating in roving tabindex and keyboard navigation.
/// </summary>
internal interface IBradixMenuRovingItem
{
    /// <summary>Gets the stable tab-stop identifier for this item.</summary>
    string TabStopId { get; }

    /// <summary>Gets a value indicating whether the item is disabled for interaction.</summary>
    bool IsDisabled { get; }

    /// <summary>Gets the text value used for typeahead matching.</summary>
    string TextValue { get; }

    /// <summary>Moves keyboard focus to this item without scrolling the page.</summary>
    ValueTask Focus();
}
