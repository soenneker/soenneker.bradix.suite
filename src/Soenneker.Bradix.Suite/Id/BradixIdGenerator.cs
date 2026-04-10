using System.Threading;

namespace Soenneker.Bradix;

/// <summary>
/// Scoped id generator for Bradix primitives.
/// </summary>
public sealed class BradixIdGenerator : IBradixIdGenerator
{
    private long _count;

    public string New(string prefix)
    {
        long next = Interlocked.Increment(ref _count);
        return $"{prefix}-{next}";
    }
}
