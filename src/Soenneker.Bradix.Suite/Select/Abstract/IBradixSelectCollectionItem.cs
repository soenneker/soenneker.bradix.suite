using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Internal contract implemented by select items for registration and interaction with <see cref="BradixSelect"/>.
/// </summary>
internal interface IBradixSelectCollectionItem
{
    string Value { get; }

    bool Disabled { get; }

    string TextValue { get; }

    ElementReference ItemElement { get; }

    ElementReference ItemTextElement { get; }

    bool HasItemTextElement { get; }

    ValueTask ScrollIntoView();

    ValueTask Focus();

    ValueTask Refresh();
}
