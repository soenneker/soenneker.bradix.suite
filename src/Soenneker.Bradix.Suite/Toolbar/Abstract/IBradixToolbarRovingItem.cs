using System.Threading.Tasks;

namespace Soenneker.Bradix;

/// <summary>
/// Internal contract for toolbar controls that participate in roving tabindex navigation.
/// </summary>
internal interface IBradixToolbarRovingItem
{
    string? TabStopId { get; }

    bool IsDisabled { get; }

    ValueTask Focus();
}
