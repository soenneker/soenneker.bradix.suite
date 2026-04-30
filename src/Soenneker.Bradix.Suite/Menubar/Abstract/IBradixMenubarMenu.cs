using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>Defines the public API for <see cref="BradixMenubarMenu"/>.</summary>
public interface IBradixMenubarMenu
{
    /// <summary>Gets or sets the value identifying this menu within the menubar.</summary>
    string? Value { get; set; }

    /// <summary>Gets the base id used for stable trigger and content ids.</summary>
    string? BaseId { get; }
/// <summary>Gets or sets the child content.</summary>
    RenderFragment? ChildContent { get; set; }
}
