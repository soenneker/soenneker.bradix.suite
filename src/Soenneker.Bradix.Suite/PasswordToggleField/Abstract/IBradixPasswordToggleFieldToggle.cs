using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Soenneker.Bradix;

/// <summary>Defines the password visibility Toggle API.</summary>
public interface IBradixPasswordToggleFieldToggle : IAsyncDisposable
{
    /// <summary>Whether the toggle is disabled.</summary>
    bool Disabled { get; set; }

    /// <summary>Optional click callback.</summary>
    EventCallback<MouseEventArgs> OnClick { get; set; }

}
