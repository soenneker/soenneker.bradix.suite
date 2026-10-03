using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;

namespace Soenneker.Bradix;

internal sealed class BradixNavigationMenuRootLinkRegistration(string value, string triggerId, ElementReference triggerElement) : IBradixNavigationMenuRegisteredTrigger
{
    public string Value { get; } = value;
    public string TriggerId { get; } = triggerId;
    public bool Disabled => false;
    public ElementReference TriggerElement { get; } = triggerElement;

    public ValueTask Focus()
    {
        return TriggerElement.FocusAsync();
    }
}

