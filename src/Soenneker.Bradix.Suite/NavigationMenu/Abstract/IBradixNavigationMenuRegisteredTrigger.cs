using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Registration surface for a navigation menu trigger element managed by the menu root.
/// </summary>
internal interface IBradixNavigationMenuRegisteredTrigger
{
    /// <summary>Trigger value key.</summary>
    string Value { get; }

    /// <summary>DOM id of the trigger element.</summary>
    string TriggerId { get; }

    /// <summary>Whether the trigger is disabled.</summary>
    bool Disabled { get; }

    /// <summary>Reference to the trigger element.</summary>
    ElementReference TriggerElement { get; }

    /// <summary>Moves focus to the trigger element.</summary>
    ValueTask FocusAsync();
}
