using System;
using System.Threading.Tasks;

namespace Soenneker.Bradix;

internal sealed class BradixMenuRadioGroupContext
{
    public required string? CurrentValue { get; init; }
    public required bool Disabled { get; init; }
    public required Func<string, Task> SetValue { get; init; }
}
