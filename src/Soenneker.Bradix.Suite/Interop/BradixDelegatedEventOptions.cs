namespace Soenneker.Bradix;

/// <summary>Configuration for one delegated browser event. Null values preserve JavaScript defaults.</summary>
public sealed class BradixDelegatedEventOptions
{
    /// <summary>Gets or sets the event callback name.</summary>
    public string? Method { get; set; }

    /// <summary>Gets or sets whether only events targeting the registered element are handled.</summary>
    public bool? CurrentTargetOnly { get; set; }

    /// <summary>Gets or sets whether previously prevented events are ignored.</summary>
    public bool? CheckForDefaultPrevented { get; set; }

    /// <summary>Gets or sets the keys to handle.</summary>
    public string[]? Keys { get; set; }

    /// <summary>Gets or sets the keys whose default behavior is prevented.</summary>
    public string[]? PreventDefaultKeys { get; set; }

    /// <summary>Gets or sets whether to prevent default behavior.</summary>
    public bool? PreventDefault { get; set; }

    /// <summary>Gets or sets whether to stop event propagation.</summary>
    public bool? StopPropagation { get; set; }

    /// <summary>Gets or sets the pointer event filter.</summary>
    public string? Filter { get; set; }

    /// <summary>Gets or sets whether pointer release is retargeted to a select option.</summary>
    public bool? RetargetPointerUpToOption { get; set; }

    /// <summary>Gets or sets the pointer selection callback name.</summary>
    public string? PointerSelectMethod { get; set; }
}
