using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Soenneker.Bradix;

/// <summary>Defines the password visibility root API.</summary>
public interface IBradixPasswordToggleField
{
    /// <summary>Controlled visibility; omit to use internal state.</summary>
    bool? Visible { get; set; }

    /// <summary>Initial visibility for an uncontrolled field.</summary>
    bool DefaultVisible { get; set; }

    /// <summary>Callback for two-way visibility binding.</summary>
    EventCallback<bool> VisibleChanged { get; set; }

    /// <summary>Invoked when visibility is requested to change.</summary>
    EventCallback<bool> OnVisibilityChange { get; set; }

    /// <summary>The composed field parts.</summary>
    RenderFragment? ChildContent { get; set; }

}
