using System;

namespace Soenneker.Bradix.Suite.Tests;

internal sealed class BradixTypeaheadManualTimeProvider : TimeProvider
{
    private DateTimeOffset _utcNow;

    public BradixTypeaheadManualTimeProvider(DateTimeOffset utcNow)
    {
        _utcNow = utcNow;
    }

    public override DateTimeOffset GetUtcNow()
    {
        return _utcNow;
    }

    public void Advance(TimeSpan duration)
    {
        _utcNow = _utcNow.Add(duration);
    }
}
