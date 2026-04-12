using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Renders a fragment with the current validity snapshot for a field.
/// </summary>
public interface IBradixFormValidityState
{
    /// <summary>Field name override; defaults to the cascading field name.</summary>
    string? Name { get; set; }

    /// <summary>Fragment receiving the resolved validity snapshot.</summary>
    RenderFragment<BradixFormValiditySnapshot?>? ChildContent { get; set; }
}
