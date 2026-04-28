using System;

namespace Soenneker.Bradix;

/// <summary>
/// Describes the public API of <see cref="BradixDialogTrigger"/>.
/// </summary>
public interface IBradixDialogTrigger {
    /// <summary>
    /// Gets or sets whether the trigger is disabled.
    /// </summary>
    bool Disabled { get; set; }
}
