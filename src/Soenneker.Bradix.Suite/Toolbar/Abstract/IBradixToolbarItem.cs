using System.Threading.Tasks;

namespace Soenneker.Bradix;

internal interface IBradixToolbarItem
{
    string? TabStopId { get; }
    bool IsDisabled { get; }
    ValueTask FocusAsync();
}
