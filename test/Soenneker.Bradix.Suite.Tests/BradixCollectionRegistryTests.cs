using System.Linq;
using System.Threading.Tasks;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixCollectionRegistryTests
{
    [Test]
    public async Task Snapshot_returns_keys_in_registration_order()
    {
        var registry = new BradixCollectionRegistry<DemoItem>();

        registry.Register("alpha", new DemoItem("Alpha"));
        registry.Register("beta", new DemoItem("Beta"));
        registry.Register("blue", new DemoItem("Blue"));

        await Assert.That(string.Join(",", registry.Snapshot().Select(entry => entry.Key))).IsEqualTo("alpha,beta,blue");
    }

    [Test]
    public async Task Insert_repositions_existing_entry_without_duplication()
    {
        var registry = new BradixCollectionRegistry<DemoItem>();

        registry.Register("alpha", new DemoItem("Alpha"));
        registry.Register("beta", new DemoItem("Beta"));
        registry.Register("blue", new DemoItem("Blue"));
        registry.Insert(0, "blue", new DemoItem("Blue"));

        BradixCollectionEntry<DemoItem>[] snapshot = [.. registry.Snapshot()];

        await Assert.That(string.Join(",", snapshot.Select(entry => entry.Key))).IsEqualTo("blue,alpha,beta");
    }

    [Test]
    public async Task Ordered_dictionary_invalid_lookup_does_not_return_default_key_item()
    {
        var dictionary = new BradixOrderedDictionary<int, string>();

        dictionary.Set(0, "Zero");

        await Assert.That(dictionary.At(1)).IsNull();
        await Assert.That(dictionary.EntryAt(1)).IsNull();
    }

    [Test]
    public async Task Ordered_dictionary_matches_radix_negative_insert_semantics()
    {
        var dictionary = new BradixOrderedDictionary<string, string>();

        dictionary.Set("alpha", "Alpha");
        dictionary.Set("beta", "Beta");
        dictionary.Set("blue", "Blue");
        dictionary.Insert(-2, "amber", "Amber");

        await Assert.That(string.Join(",", dictionary.Select(entry => entry.Key))).IsEqualTo("alpha,beta,amber,blue");
    }

    [Test]
    public async Task Ordered_dictionary_repositions_existing_key_without_shifting_values()
    {
        var dictionary = new BradixOrderedDictionary<string, string>();

        dictionary.Set("alpha", "Alpha");
        dictionary.Set("beta", "Beta");
        dictionary.Set("cyan", "Cyan");
        dictionary.Set("delta", "Delta");

        dictionary.Insert(0, "cyan", "Cyan updated");

        await Assert.That(string.Join(",", dictionary.Select(entry => entry.Key))).IsEqualTo("cyan,alpha,beta,delta");
        await Assert.That(dictionary["cyan"]).IsEqualTo("Cyan updated");
        await Assert.That(dictionary["alpha"]).IsEqualTo("Alpha");
        await Assert.That(dictionary["beta"]).IsEqualTo("Beta");
        await Assert.That(dictionary["delta"]).IsEqualTo("Delta");
    }

    [Test]
    public async Task Ordered_dictionary_out_of_range_insert_updates_without_reordering_existing_key()
    {
        var dictionary = new BradixOrderedDictionary<string, string>();

        dictionary.Set("alpha", "Alpha");
        dictionary.Set("beta", "Beta");
        dictionary.Set("cyan", "Cyan");

        dictionary.Insert(99, "beta", "Beta updated");
        dictionary.Insert(-99, "cyan", "Cyan updated");

        await Assert.That(string.Join(",", dictionary.Select(entry => entry.Key))).IsEqualTo("alpha,beta,cyan");
        await Assert.That(dictionary["beta"]).IsEqualTo("Beta updated");
        await Assert.That(dictionary["cyan"]).IsEqualTo("Cyan updated");
    }

    [Test]
    public async Task Ordered_dictionary_out_of_range_insert_appends_new_key()
    {
        var dictionary = new BradixOrderedDictionary<string, string>();

        dictionary.Set("alpha", "Alpha");
        dictionary.Set("beta", "Beta");

        dictionary.Insert(99, "cyan", "Cyan");
        dictionary.Insert(-99, "amber", "Amber");

        await Assert.That(string.Join(",", dictionary.Select(entry => entry.Key))).IsEqualTo("alpha,beta,cyan,amber");
    }

    [Test]
    public async Task Ordered_dictionary_supports_radix_relative_helpers()
    {
        var dictionary = new BradixOrderedDictionary<string, string>();

        dictionary.Set("alpha", "Alpha");
        dictionary.Set("blue", "Blue");
        dictionary.SetBefore("blue", "amber", "Amber");
        dictionary.SetAfter("blue", "cyan", "Cyan");

        await Assert.That(dictionary.First()?.Key).IsEqualTo("alpha");
        await Assert.That(dictionary.Last()?.Key).IsEqualTo("cyan");
        await Assert.That(dictionary.Before("blue")?.Key).IsEqualTo("amber");
        await Assert.That(dictionary.After("blue")?.Key).IsEqualTo("cyan");
        await Assert.That(dictionary.KeyFrom("alpha", 2)).IsEqualTo("blue");
    }

    [Test]
    public async Task Ordered_dictionary_filter_reverse_and_delete_at_preserve_order()
    {
        var dictionary = new BradixOrderedDictionary<string, string>();

        dictionary.Set("alpha", "Alpha");
        dictionary.Set("amber", "Amber");
        dictionary.Set("beta", "Beta");
        dictionary.Set("blue", "Blue");

        BradixOrderedDictionary<string, string> filtered = dictionary.Filter(entry => entry.Key.StartsWith('b'));
        BradixOrderedDictionary<string, string> reversed = filtered.ToReversed();

        await Assert.That(string.Join(",", filtered.Select(entry => entry.Key))).IsEqualTo("beta,blue");
        await Assert.That(string.Join(",", reversed.Select(entry => entry.Key))).IsEqualTo("blue,beta");

        await Assert.That(dictionary.DeleteAt(-1)).IsTrue();
        await Assert.That(string.Join(",", dictionary.Select(entry => entry.Key))).IsEqualTo("alpha,amber,beta");
    }

    private sealed record DemoItem(string TextValue);
}
