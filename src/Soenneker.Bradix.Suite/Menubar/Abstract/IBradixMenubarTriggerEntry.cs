using System.Threading.Tasks;

namespace Soenneker.Bradix;

/// <summary>
/// Internal contract for menubar triggers registered for roving focus and keyboard navigation.
/// </summary>
internal interface IBradixMenubarTriggerEntry
{
    /// <summary>Gets the value identifying this menubar menu.</summary>
    string Value { get; }

    /// <summary>Gets the DOM id of the trigger element.</summary>
    string TriggerId { get; }

    /// <summary>Gets a value indicating whether the trigger is disabled.</summary>
    bool Disabled { get; }

    /// <summary>Moves focus to the trigger button.</summary>
    ValueTask FocusAsync();
}
