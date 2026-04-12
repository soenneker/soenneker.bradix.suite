using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Soenneker.Bradix;

/// <summary>
/// Describes the public API of <see cref="BradixCollapsibleTrigger"/>.
/// </summary>
public interface IBradixCollapsibleTrigger
{
    /// <summary>
    /// Gets or sets the callback invoked when the trigger is clicked.
    /// </summary>
    EventCallback<MouseEventArgs> OnClick { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when a key is pressed on the trigger.
    /// </summary>
    EventCallback<KeyboardEventArgs> OnKeyDown { get; set; }

    /// <summary>
    /// Gets or sets the <c>aria-disabled</c> attribute value for the trigger.
    /// </summary>
    string? AriaDisabled { get; set; }

    /// <summary>
    /// Releases resources used by delegated interaction registration.
    /// </summary>
    ValueTask DisposeAsync();

    /// <summary>
    /// Called when delegated interaction handling is ready on the trigger element.
    /// </summary>
    Task HandleDelegatedInteractionReady();

    /// <summary>
    /// Handles a delegated click routed from JavaScript.
    /// </summary>
    /// <param name="mouseEvent">The delegated mouse event payload.</param>
    Task HandleDelegatedClick(BradixDelegatedMouseEvent mouseEvent);
}
