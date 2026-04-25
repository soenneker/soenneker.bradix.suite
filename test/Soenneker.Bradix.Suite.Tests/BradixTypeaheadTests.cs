using System;
using System.Threading.Tasks;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixTypeaheadTests
{
    [Test]
    public async Task Buffer_expires_after_idle_timeout()
    {
        var timeProvider = new ManualTimeProvider(new DateTimeOffset(2026, 4, 9, 12, 0, 0, TimeSpan.Zero));
        var buffer = new BradixTypeaheadBuffer(timeProvider);

        buffer.Append("a");
        timeProvider.Advance(TimeSpan.FromMilliseconds(999));

        await Assert.That(buffer.CurrentSearch).IsEqualTo("a");

        timeProvider.Advance(TimeSpan.FromMilliseconds(1));

        await Assert.That(buffer.CurrentSearch).IsEqualTo(string.Empty);
    }

    [Test]
    public async Task Single_character_search_skips_the_current_match()
    {
        string? next = BradixTypeaheadMatcher.FindNextMatch(["Alpha", "Amber", "Beta"], "a", "Alpha");

        await Assert.That(next).IsEqualTo("Amber");
    }

    [Test]
    public async Task Repeated_character_search_normalizes_to_single_character_cycle()
    {
        string? next = BradixTypeaheadMatcher.FindNextMatch(["Alpha", "Amber", "Beta"], "aaa", "Alpha");

        await Assert.That(next).IsEqualTo("Amber");
    }

    [Test]
    public async Task Repeated_supplementary_plane_search_matches_radix_string_iterator_behavior()
    {
        var emoji = char.ConvertFromUtf32(0x1F600);
        string? next = BradixTypeaheadMatcher.FindNextMatch([$"{emoji} single", $"{emoji}{emoji} repeated"], $"{emoji}{emoji}");

        await Assert.That(next).IsEqualTo($"{emoji}{emoji} repeated");
    }

    [Test]
    public async Task Single_supplementary_plane_search_does_not_exclude_current_match_like_radix()
    {
        var emoji = char.ConvertFromUtf32(0x1F600);
        string? next = BradixTypeaheadMatcher.FindNextMatch([$"{emoji} Alpha", $"{emoji} Beta"], emoji, $"{emoji} Alpha");

        await Assert.That(next).IsNull();
    }

    [Test]
    public async Task Multi_character_search_can_leave_focus_on_current_match()
    {
        string? next = BradixTypeaheadMatcher.FindNextMatch(["Alpha", "Amber", "Beta"], "al", "Alpha");

        await Assert.That(next).IsNull();
    }

    [Test]
    public async Task Multi_character_search_returns_null_when_no_item_matches()
    {
        string? next = BradixTypeaheadMatcher.FindNextMatch(["Alpha", "Amber", "Beta"], "zz", "Alpha");

        await Assert.That(next).IsNull();
    }

    [Test]
    public async Task Whitespace_search_is_preserved_like_radix()
    {
        string? next = BradixTypeaheadMatcher.FindNextMatch(["Alpha", " Alpha"], " ");

        await Assert.That(next).IsEqualTo(" Alpha");
    }

    [Test]
    public async Task Generic_matcher_returns_next_item_in_wrapped_order()
    {
        DemoItem[] items =
        [
            new DemoItem("Alpha"),
            new DemoItem("Beta"),
            new DemoItem("Blue")
        ];

        DemoItem? next = BradixTypeaheadMatcher.FindNextItem(items, "b", items[1], item => item.TextValue);

        await Assert.That(next?.TextValue).IsEqualTo("Blue");
    }

    [Test]
    public async Task Generic_matcher_preserves_explicit_text_value_whitespace()
    {
        DemoItem[] items =
        [
            new DemoItem("Alpha"),
            new DemoItem(" Alpha")
        ];

        DemoItem? next = BradixTypeaheadMatcher.FindNextItem(items, " ", null, item => item.TextValue);

        await Assert.That(next?.TextValue).IsEqualTo(" Alpha");
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
