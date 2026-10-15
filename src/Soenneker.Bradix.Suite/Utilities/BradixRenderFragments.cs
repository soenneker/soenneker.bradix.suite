using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

internal static class BradixRenderFragments
{
    // Forwarding wrappers must still supply non-null content when no content was
    // specified, since downstream primitives can distinguish null from empty.
    internal static readonly RenderFragment Empty = static _ => { };
}
