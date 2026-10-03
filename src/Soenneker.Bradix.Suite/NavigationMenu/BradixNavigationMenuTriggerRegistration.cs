using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;

namespace Soenneker.Bradix;

internal sealed class BradixNavigationMenuTriggerRegistration(string value, string triggerId, BradixNavigationMenuTrigger owner) : IBradixNavigationMenuRegisteredTrigger
{
    public string Value { get; } = value;
    public string TriggerId { get; } = triggerId;
    public bool Disabled => owner.Disabled;
    public ElementReference TriggerElement => owner.TriggerElement;

    public ValueTask Focus()
    {
        return TriggerElement.FocusAsync();
    }
}
