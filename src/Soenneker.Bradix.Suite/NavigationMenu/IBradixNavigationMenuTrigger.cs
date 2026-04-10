using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

internal interface IBradixNavigationMenuTrigger
{
    string Value { get; }
    string TriggerId { get; }
    bool Disabled { get; }
    ElementReference TriggerElement { get; }
    ValueTask FocusAsync();
}
