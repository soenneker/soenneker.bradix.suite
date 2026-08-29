using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Soenneker.Bradix;

/// <summary>
/// Describes the public API of <see cref="BradixDialogClose"/>.
/// </summary>
public interface IBradixDialogClose : IAsyncDisposable {
    /// <summary>
    /// Gets or sets the callback invoked when the close button element reference is available.
    /// </summary>
    EventCallback<ElementReference> OnElementReferenceCaptured { get; set; }

    /// <summary>
    /// Gets or sets the accessible label for the close button.
    /// </summary>
    string? AriaLabel { get; set; }

    /// <summary>
    /// Gets or sets whether the close button is disabled.
    /// </summary>
    bool Disabled { get; set; }

    /// <summary>
    /// Gets or sets whether the close behavior is composed onto the child content.
    /// </summary>
    bool AsChild { get; set; }

    /// <summary>
    /// Gets or sets the element name rendered when <see cref="AsChild"/> is true and Bradix renders the slotted element.
    /// </summary>
    string? ChildElementName { get; set; }

    /// <summary>
    /// Gets or sets attributes from the child element that should be merged into the slotted close element.
    /// </summary>
    IReadOnlyDictionary<string, object>? ChildAttributes { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when the close button is clicked.
    /// </summary>
    EventCallback<MouseEventArgs> OnClick { get; set; }

    /// <summary>
    /// Called when delegated interaction handling is ready on the close button.
    /// </summary>
    /// <returns>A task that completes when the handle delegated interaction ready operation is complete.</returns>
    Task HandleDelegatedInteractionReady();

    /// <summary>
    /// Handles a delegated click routed from JavaScript.
    /// </summary>
    /// <param name="mouseEvent">Mouse Event for the handle delegated click operation.</param>
    /// <returns>A task that completes when the handle delegated click operation is complete.</returns>
    Task HandleDelegatedClick(BradixDelegatedMouseEvent mouseEvent);
}
