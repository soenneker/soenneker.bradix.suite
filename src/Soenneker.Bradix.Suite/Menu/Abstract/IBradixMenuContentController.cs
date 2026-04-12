using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Web;

namespace Soenneker.Bradix;

internal interface IBradixMenuContentController
{
    Task HandleItemKeyDownAsync(IBradixMenuRovingItem item, KeyboardEventArgs args);
    Task HandleTypeaheadKeyAsync(IBradixMenuRovingItem item, string key);
    Task HandlePointerMoveAsync(IBradixMenuRovingItem item, string? pointerType);
    Task HandleItemLeaveAsync();
    void SetSubmenuPointerGrace(string? tabStopId, bool active);
}
