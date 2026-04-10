using Soenneker.Bradix.Suite.Typeahead;
using System;
using Xunit;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixTypeaheadTests
{
    [Fact]
    public void Buffer_expires_after_idle_timeout()
    {
        var timeProvider = new ManualTimeProvider(new DateTimeOffset(2026, 4, 9, 12, 0, 0, TimeSpan.Zero));
        var buffer = new BradixTypeaheadBuffer(timeProvider);

        buffer.Append("a");
        timeProvider.Advance(TimeSpan.FromMilliseconds(999));

        Assert.Equal("a", buffer.CurrentSearch);

        timeProvider.Advance(TimeSpan.FromMilliseconds(1));

        Assert.Equal(string.Empty, buffer.CurrentSearch);
    }

    [Fact]
    public void Single_character_search_skips_the_current_match()
    {
        string? next = BradixTypeaheadMatcher.FindNextMatch(["Alpha", "Amber", "Beta"], "a", "Alpha");

        Assert.Equal("Amber", next);
    }

    [Fact]
    public void Multi_character_search_can_leave_focus_on_current_match()
    {
        string? next = BradixTypeaheadMatcher.FindNextMatch(["Alpha", "Amber", "Beta"], "al", "Alpha");

        Assert.Null(next);
    }

    [Fact]
    public void Generic_matcher_returns_next_item_in_wrapped_order()
    {
        DemoItem[] items =
        [
            new DemoItem("Alpha"),
            new DemoItem("Beta"),
            new DemoItem("Blue")
        ];

        DemoItem? next = BradixTypeaheadMatcher.FindNextItem(items, "b", items[1], item => item.TextValue);

        Assert.Equal("Blue", next?.TextValue);
    }

    private sealed record DemoItem(string TextValue);

    private sealed class ManualTimeProvider : TimeProvider
    {
        private DateTimeOffset _utcNow;

        public ManualTimeProvider(DateTimeOffset utcNow)
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
}
