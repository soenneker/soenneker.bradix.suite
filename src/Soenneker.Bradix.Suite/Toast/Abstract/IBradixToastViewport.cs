using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Defines the public API for <see cref="BradixToastViewport"/>.
/// </summary>
public interface IBradixToastViewport : IAsyncDisposable {
    /// <summary>Gets or sets the element identifier.</summary>
    string? Id { get; set; }

    /// <summary>Gets or sets the CSS class.</summary>
    string? Class { get; set; }

    /// <summary>Gets or sets the inline style.</summary>
    string? Style { get; set; }

    /// <summary>Gets or sets the child content.</summary>
    RenderFragment? ChildContent { get; set; }

    /// <summary>Gets or sets additional attributes merged onto the root element.</summary>
    IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>Gets or sets the keyboard shortcut keys that focus the viewport.</summary>
    IReadOnlyList<string> Hotkey { get; set; }

    /// <summary>Gets or sets the region label; may include a <c>{hotkey}</c> token.</summary>
    string Label { get; set; }


    /// <summary>Called from script when the pause hotkey chord is pressed.</summary>
    Task HandlePause();

    /// <summary>Called from script when the resume hotkey chord is pressed.</summary>
    Task HandleResume();
}
