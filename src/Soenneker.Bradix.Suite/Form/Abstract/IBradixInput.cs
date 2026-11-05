using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>Adapts typed primitive bindings to Blazor InputBase, including ValueExpression and EditContext validation.</summary>
public interface IBradixInput<TValue>
{
    /// <summary>Renders a primitive using the supplied binding and validation attributes. No wrapper element is added.</summary>
    RenderFragment<BradixInputContext<TValue>>? ChildContent { get; set; }
}
