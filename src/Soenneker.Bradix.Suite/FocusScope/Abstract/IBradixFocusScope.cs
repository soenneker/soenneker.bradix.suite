using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Traps and restores focus within a subtree, with optional looping tab navigation.
/// </summary>
public interface IBradixFocusScope
{
    /// <summary>When true, focus cycles from last to first tab stop and vice versa.</summary>
    bool Loop { get; set; }

    /// <summary>When true, focus cannot leave the scope via Tab navigation.</summary>
    bool Trapped { get; set; }

    /// <summary>Raised when the scope mounts and default autofocus would run.</summary>
    EventCallback OnMountAutoFocus { get; set; }

    /// <summary>Raised when the scope mounts with detailed autofocus arguments.</summary>
    EventCallback<BradixAutoFocusEventArgs> OnMountAutoFocusDetailed { get; set; }

    /// <summary>Raised when the scope unmounts and default focus restore would run.</summary>
    EventCallback OnUnmountAutoFocus { get; set; }

    /// <summary>Raised when the scope unmounts with detailed autofocus arguments.</summary>
    EventCallback<BradixAutoFocusEventArgs> OnUnmountAutoFocusDetailed { get; set; }

    /// <summary>When true, suppresses the default mount autofocus behavior.</summary>
    bool PreventMountAutoFocus { get; set; }

    /// <summary>When true, suppresses the default unmount focus restore behavior.</summary>
    bool PreventUnmountAutoFocus { get; set; }

    /// <summary>Root element id.</summary>
    string? Id { get; set; }

    /// <summary>CSS class names merged onto the root element.</summary>
    string? Class { get; set; }

    /// <summary>Inline style for the root element.</summary>
    string? Style { get; set; }

    /// <summary>Child content inside the focus scope.</summary>
    RenderFragment? ChildContent { get; set; }

    /// <summary>Additional unmatched attributes applied to the root element.</summary>
    IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>Unregisters the focus scope from interop.</summary>
    ValueTask DisposeAsync();

    /// <summary>Interop handler for mount autofocus.</summary>
    Task<bool> HandleMountAutoFocus();

    /// <summary>Interop handler for unmount autofocus.</summary>
    Task<bool> HandleUnmountAutoFocus();
}
