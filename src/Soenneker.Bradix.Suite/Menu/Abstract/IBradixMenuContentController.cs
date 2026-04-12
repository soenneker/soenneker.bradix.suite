using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Web;

namespace Soenneker.Bradix;

internal interface IBradixMenuContentController
{
    Task HandleItemKeyDown(IBradixMenuRovingItem item, KeyboardEventArgs args);
    Task HandleTypeaheadKey(IBradixMenuRovingItem item, string key);
    Task HandlePointerMove(IBradixMenuRovingItem item, string? pointerType);
    Task HandleItemLeave();
    void SetSubmenuPointerGrace(string? tabStopId, bool active);
}
