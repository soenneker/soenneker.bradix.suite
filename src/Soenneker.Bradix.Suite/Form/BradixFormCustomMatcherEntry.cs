using System;
using System.Threading.Tasks;

namespace Soenneker.Bradix.Suite.Form;

internal sealed class BradixFormCustomMatcherEntry
{
    public required string Id { get; init; }

    public required Func<string?, BradixFormDataSnapshot, ValueTask<bool>> MatchAsync { get; init; }
}
