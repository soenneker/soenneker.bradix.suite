using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Soenneker.Bradix;

/// <summary>Defines the password visibility Slot API.</summary>
public interface IBradixPasswordToggleFieldSlot
{
    /// <summary>Content displayed while the password is visible.</summary>
    RenderFragment? Visible { get; set; }

    /// <summary>Content displayed while the password is hidden.</summary>
    RenderFragment? Hidden { get; set; }

    /// <summary>Optional template receiving the current visibility.</summary>
    RenderFragment<bool>? ChildContent { get; set; }

}
