using Soenneker.Bradix.Suite.Checkbox;

namespace Soenneker.Bradix.Suite.Menu;

internal sealed class BradixMenuItemIndicatorContext
{
    public required BradixCheckboxCheckedState CheckedState { get; init; }
    public required bool Disabled { get; init; }
}
