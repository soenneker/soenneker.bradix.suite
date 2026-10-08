using System;
using System.Threading.Tasks;
using System.Threading;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixTypeaheadTests
{
    [Test]
    public async ValueTask Buffer_expires_after_idle_timeout(CancellationToken cancellationToken)
    {
        var timeProvider = new BradixTypeaheadManualTimeProvider(new DateTimeOffset(2026, 4, 9, 12, 0, 0, TimeSpan.Zero));
        var buffer = new BradixTypeaheadBuffer(timeProvider);

        buffer.Append("a");
        timeProvider.Advance(TimeSpan.FromMilliseconds(999));

        await Assert.That(buffer.CurrentSearch).IsEqualTo("a");

        timeProvider.Advance(TimeSpan.FromMilliseconds(1));

        await Assert.That(buffer.CurrentSearch).IsEqualTo(string.Empty);
    }

    [Test]
    public async ValueTask Single_character_search_skips_the_current_match(CancellationToken cancellationToken)
    {
        string? next = BradixTypeaheadMatcher.FindNextMatch(["Alpha", "Amber", "Beta"], "a", "Alpha");

        await Assert.That(next).IsEqualTo("Amber");
    }

    [Test]
    public async ValueTask Repeated_character_search_normalizes_to_single_character_cycle(CancellationToken cancellationToken)
    {
        string? next = BradixTypeaheadMatcher.FindNextMatch(["Alpha", "Amber", "Beta"], "aaa", "Alpha");

        await Assert.That(next).IsEqualTo("Amber");
    }

    [Test]
    public async ValueTask Repeated_supplementary_plane_search_matches_radix_string_iterator_behavior(CancellationToken cancellationToken)
    {
        string emoji = char.ConvertFromUtf32(0x1F600);
        string? next = BradixTypeaheadMatcher.FindNextMatch([$"{emoji} single", $"{emoji}{emoji} repeated"], $"{emoji}{emoji}");

        await Assert.That(next).IsEqualTo($"{emoji}{emoji} repeated");
    }

    [Test]
    public async ValueTask Single_supplementary_plane_search_does_not_exclude_current_match_like_radix(CancellationToken cancellationToken)
    {
        string emoji = char.ConvertFromUtf32(0x1F600);
        string? next = BradixTypeaheadMatcher.FindNextMatch([$"{emoji} Alpha", $"{emoji} Beta"], emoji, $"{emoji} Alpha");

        await Assert.That(next).IsNull();
    }

    [Test]
    public async ValueTask Multi_character_search_can_leave_focus_on_current_match(CancellationToken cancellationToken)
    {
        string? next = BradixTypeaheadMatcher.FindNextMatch(["Alpha", "Amber", "Beta"], "al", "Alpha");

        await Assert.That(next).IsNull();
    }

    [Test]
    public async ValueTask Multi_character_search_returns_null_when_no_item_matches(CancellationToken cancellationToken)
    {
        string? next = BradixTypeaheadMatcher.FindNextMatch(["Alpha", "Amber", "Beta"], "zz", "Alpha");

        await Assert.That(next).IsNull();
    }

    [Test]
    public async ValueTask Whitespace_search_is_preserved_like_radix(CancellationToken cancellationToken)
    {
        string? next = BradixTypeaheadMatcher.FindNextMatch(["Alpha", " Alpha"], " ");

        await Assert.That(next).IsEqualTo(" Alpha");
    }

    [Test]
    public async ValueTask Generic_matcher_returns_next_item_in_wrapped_order(CancellationToken cancellationToken)
    {
        BradixTypeaheadDemoItem[] items =
        [
            new BradixTypeaheadDemoItem("Alpha"),
            new BradixTypeaheadDemoItem("Beta"),
            new BradixTypeaheadDemoItem("Blue")
        ];

        BradixTypeaheadDemoItem? next = BradixTypeaheadMatcher.FindNextItem(items, "b", items[1], item => item.TextValue);

        await Assert.That(next?.TextValue).IsEqualTo("Blue");
    }

    [Test]
    public async ValueTask Generic_matcher_preserves_explicit_text_value_whitespace(CancellationToken cancellationToken)
    {
        BradixTypeaheadDemoItem[] items =
        [
            new BradixTypeaheadDemoItem("Alpha"),
            new BradixTypeaheadDemoItem(" Alpha")
        ];

        BradixTypeaheadDemoItem? next = BradixTypeaheadMatcher.FindNextItem(items, " ", null, item => item.TextValue);

        await Assert.That(next?.TextValue).IsEqualTo(" Alpha");
    }

}
