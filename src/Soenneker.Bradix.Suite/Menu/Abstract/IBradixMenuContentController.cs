using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Web;

namespace Soenneker.Bradix;

internal interface IBradixMenuContentController
{
    Task HandleItemKeyDownAsync(IBradixMenuItem item, KeyboardEventArgs args);
    Task HandleTypeaheadKeyAsync(IBradixMenuItem item, string key);
    Task HandlePointerMoveAsync(IBradixMenuItem item, string? pointerType);
    Task HandleItemLeaveAsync();
    void SetSubmenuPointerGrace(string? tabStopId, bool active);
}
