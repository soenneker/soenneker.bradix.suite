using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Web;

namespace Soenneker.Bradix;

public sealed class BradixDialogCloseContext
{
    public bool Disabled { get; init; }

    public string? AriaLabel { get; init; }

    public Func<MouseEventArgs, Task> Close { get; init; } = _ => Task.CompletedTask;
}
