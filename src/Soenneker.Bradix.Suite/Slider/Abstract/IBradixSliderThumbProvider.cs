using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>Provides a slider thumb's state to explicitly composed trigger and form-input parts.</summary>
public interface IBradixSliderThumbProvider : IBradixSliderThumb
{
    /// <summary>Gets or sets the thumb parts.</summary>
    RenderFragment? ChildContent { get; set; }
}
