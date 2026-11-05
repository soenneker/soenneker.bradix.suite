using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>Shared customization contract for click-activated triggers.</summary>
public interface IBradixComposableTrigger
{
    /// <summary>Optional replacement for the default button. Forward the context's attributes, events, and element reference to a focusable control.</summary>
    RenderFragment<BradixTriggerContext>? TriggerTemplate { get; set; }

    /// <summary>Runs after OnClick and before the default action. Call PreventDefault to cancel the state change; this does not cancel browser default actions.</summary>
    EventCallback<BradixActivationEventArgs> OnActivate { get; set; }
}
