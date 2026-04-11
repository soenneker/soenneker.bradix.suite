using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

internal interface IBradixSelectItem
{
    string Value { get; }

    bool Disabled { get; }

    string TextValue { get; }

    ElementReference ItemElement { get; }

    ElementReference ItemTextElement { get; }

    bool HasItemTextElement { get; }

    ValueTask ScrollIntoViewAsync();

    ValueTask FocusAsync();
}
