using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Soenneker.Bradix;

/// <summary>Defines the password visibility Input API.</summary>
public interface IBradixPasswordToggleFieldInput : IAsyncDisposable
{
    /// <summary>The input value when bound.</summary>
    string? Value { get; set; }

    /// <summary>Invoked on input when binding the value.</summary>
    EventCallback<string?> ValueChanged { get; set; }

    /// <summary>Optional input event callback.</summary>
    EventCallback<ChangeEventArgs> OnInput { get; set; }

    /// <summary>Password autocomplete hint, defaulting to current-password.</summary>
    string AutoComplete { get; set; }

    /// <summary>Form field name.</summary>
    string? Name { get; set; }

    /// <summary>Associated form ID.</summary>
    string? Form { get; set; }

    /// <summary>Whether input is disabled.</summary>
    bool Disabled { get; set; }

    /// <summary>Whether input is required.</summary>
    bool Required { get; set; }

}
