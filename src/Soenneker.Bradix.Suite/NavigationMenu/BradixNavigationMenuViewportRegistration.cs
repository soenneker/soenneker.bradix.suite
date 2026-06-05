using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace Soenneker.Bradix;

/// <summary>
/// Represents the bradix navigation menu viewport registration.
/// </summary>
public sealed class BradixNavigationMenuViewportRegistration
{
    /// <summary>
    /// Gets or sets value.
    /// </summary>
    public required string Value { get; init; }
    /// <summary>
    /// Gets or sets trigger id.
    /// </summary>
    public required string TriggerId { get; init; }
    /// <summary>
    /// Gets or sets content id.
    /// </summary>
    public required string ContentId { get; init; }
    /// <summary>
    /// Gets or sets class.
    /// </summary>
    public string? Class { get; init; }
    /// <summary>
    /// Gets or sets style.
    /// </summary>
    public string? Style { get; init; }
    /// <summary>
    /// Gets or sets additional attributes.
    /// </summary>
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; init; }
    /// <summary>
    /// Gets or sets child content.
    /// </summary>
    public RenderFragment? ChildContent { get; init; }
    /// <summary>
    /// Gets or sets a value indicating whether force mount.
    /// </summary>
    public bool ForceMount { get; init; }
}
