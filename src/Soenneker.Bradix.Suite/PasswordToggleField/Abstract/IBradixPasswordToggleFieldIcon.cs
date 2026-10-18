using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Soenneker.Bradix;

/// <summary>Defines the password visibility Icon API.</summary>
public interface IBradixPasswordToggleFieldIcon
{
    /// <summary>Decorative icon displayed while visible.</summary>
    RenderFragment? Visible { get; set; }

    /// <summary>Decorative icon displayed while hidden.</summary>
    RenderFragment? Hidden { get; set; }

}
