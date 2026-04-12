using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Describes the public API of <see cref="BradixDirectionProvider"/>.
/// </summary>
public interface IBradixDirectionProvider
{
    /// <summary>
    /// Gets or sets the text direction provided to descendants (<c>ltr</c> or <c>rtl</c>).
    /// </summary>
    string Dir { get; set; }

    /// <summary>
    /// Gets or sets the child content that receives the direction cascading value.
    /// </summary>
    RenderFragment? ChildContent { get; set; }
}
