using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Web;

namespace Soenneker.Bradix;

/// <summary>
/// Represents the bradix dialog close context.
/// </summary>
public sealed class BradixDialogCloseContext
{
    /// <summary>
    /// Gets or sets a value indicating whether disabled.
    /// </summary>
    public bool Disabled { get; init; }

    /// <summary>
    /// Gets or sets aria label.
    /// </summary>
    public string? AriaLabel { get; init; }

    /// <summary>
    /// Gets or sets close.
    /// </summary>
    public Func<MouseEventArgs, Task> Close { get; init; } = _ => Task.CompletedTask;
}
