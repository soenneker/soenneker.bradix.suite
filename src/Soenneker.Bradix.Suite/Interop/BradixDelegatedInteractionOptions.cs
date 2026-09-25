namespace Soenneker.Bradix;

/// <summary>Options for browser event delegation.</summary>
public sealed class BradixDelegatedInteractionOptions
{
    /// <summary>Gets or sets the callback invoked after registration.</summary>
    public string? ReadyMethod { get; set; }

    /// <summary>Gets or sets the click event configuration.</summary>
    public BradixDelegatedEventOptions? Click { get; set; }

    /// <summary>Gets or sets the mousedown event configuration.</summary>
    public BradixDelegatedEventOptions? Mousedown { get; set; }

    /// <summary>Gets or sets the pointerdown event configuration.</summary>
    public BradixDelegatedEventOptions? Pointerdown { get; set; }

    /// <summary>Gets or sets the mouseover event configuration.</summary>
    public BradixDelegatedEventOptions? Mouseover { get; set; }

    /// <summary>Gets or sets the mouseenter event configuration.</summary>
    public BradixDelegatedEventOptions? Mouseenter { get; set; }

    /// <summary>Gets or sets the pointermove event configuration.</summary>
    public BradixDelegatedEventOptions? Pointermove { get; set; }

    /// <summary>Gets or sets the pointerover event configuration.</summary>
    public BradixDelegatedEventOptions? Pointerover { get; set; }

    /// <summary>Gets or sets the keydown event configuration.</summary>
    public BradixDelegatedEventOptions? Keydown { get; set; }

    /// <summary>Gets or sets the focusin event configuration.</summary>
    public BradixDelegatedEventOptions? Focusin { get; set; }

    /// <summary>Gets or sets the focusout event configuration.</summary>
    public BradixDelegatedEventOptions? Focusout { get; set; }
}
