using System.Threading.Tasks;

namespace Soenneker.Bradix.Suite.Toolbar;

internal interface IBradixToolbarItem
{
    string? TabStopId { get; }
    bool IsDisabled { get; }
    ValueTask FocusAsync();
}
