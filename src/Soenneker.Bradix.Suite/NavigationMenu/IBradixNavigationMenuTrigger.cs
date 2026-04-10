using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Soenneker.Bradix.Suite.NavigationMenu;

internal interface IBradixNavigationMenuTrigger
{
    string Value { get; }
    string TriggerId { get; }
    bool Disabled { get; }
    ElementReference TriggerElement { get; }
    ValueTask FocusAsync();
}
