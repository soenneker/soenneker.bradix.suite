using System;
using System.Threading.Tasks;

namespace Soenneker.Bradix.Suite.Menu;

internal sealed class BradixMenuRadioGroupContext
{
    public required string? CurrentValue { get; init; }
    public required bool Disabled { get; init; }
    public required Func<string, Task> SetValueAsync { get; init; }
}
