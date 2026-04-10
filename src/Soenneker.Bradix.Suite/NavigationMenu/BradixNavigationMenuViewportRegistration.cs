using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace Soenneker.Bradix;

public sealed class BradixNavigationMenuViewportRegistration
{
    public required string Value { get; init; }
    public required string TriggerId { get; init; }
    public required string ContentId { get; init; }
    public string? Class { get; init; }
    public string? Style { get; init; }
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; init; }
    public RenderFragment? ChildContent { get; init; }
    public bool ForceMount { get; init; }
}
