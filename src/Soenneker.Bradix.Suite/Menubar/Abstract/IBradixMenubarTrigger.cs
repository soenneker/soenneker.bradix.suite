using System.Threading.Tasks;

namespace Soenneker.Bradix;

internal interface IBradixMenubarTrigger
{
    string Value { get; }

    string TriggerId { get; }

    bool Disabled { get; }

    ValueTask FocusAsync();
}
