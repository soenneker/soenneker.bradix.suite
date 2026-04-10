using System.Threading.Tasks;

namespace Soenneker.Bradix.Suite.Menu;

internal interface IBradixMenuItem
{
    string TabStopId { get; }
    bool IsDisabled { get; }
    string TextValue { get; }
    ValueTask FocusAsync();
}
