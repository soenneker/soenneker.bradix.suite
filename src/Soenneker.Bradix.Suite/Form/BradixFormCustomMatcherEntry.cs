using System;
using System.Threading.Tasks;

namespace Soenneker.Bradix;

internal sealed class BradixFormCustomMatcherEntry
{
    public required string Id { get; init; }

    public required Func<string?, BradixFormDataSnapshot, ValueTask<bool>> Match { get; init; }
}
