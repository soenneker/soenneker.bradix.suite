using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixCollectionRegistryTests
{
    [Test]
    public async Task Snapshot_returns_keys_in_registration_order()
    {
        var registry = new BradixCollectionRegistry<BradixCollectionRegistryDemoItem>();

        registry.Register("alpha", new BradixCollectionRegistryDemoItem("Alpha"));
        registry.Register("beta", new BradixCollectionRegistryDemoItem("Beta"));
        registry.Register("blue", new BradixCollectionRegistryDemoItem("Blue"));

        await Assert.That(string.Join(",", registry.Snapshot().Select(entry => entry.Key))).IsEqualTo("alpha,beta,blue");
    }

    [Test]
    public async Task Snapshot_is_read_only_reused_and_invalidated_by_mutations()
    {
        var registry = new BradixCollectionRegistry<BradixCollectionRegistryDemoItem>();
        registry.Register("alpha", new BradixCollectionRegistryDemoItem("Alpha"));

        IReadOnlyList<BradixCollectionEntry<BradixCollectionRegistryDemoItem>> first = registry.Snapshot();
        IReadOnlyList<BradixCollectionEntry<BradixCollectionRegistryDemoItem>> reused = registry.Snapshot();

        await Assert.That(ReferenceEquals(first, reused)).IsTrue();

        registry.SetBefore("missing", "amber", new BradixCollectionRegistryDemoItem("Amber"));
        registry.SetAfter("missing", "blue", new BradixCollectionRegistryDemoItem("Blue"));
        registry.Unregister("missing");
        await Assert.That(ReferenceEquals(first, registry.Snapshot())).IsTrue();

        var mutableView = (IList<BradixCollectionEntry<BradixCollectionRegistryDemoItem>>)first;
        await Assert.That(() =>
        {
            mutableView[0] = new BradixCollectionEntry<BradixCollectionRegistryDemoItem>("changed", new BradixCollectionRegistryDemoItem("Changed"));
        }).Throws<NotSupportedException>();

        registry.Register("beta", new BradixCollectionRegistryDemoItem("Beta"));
        IReadOnlyList<BradixCollectionEntry<BradixCollectionRegistryDemoItem>> afterRegister = registry.Snapshot();
        await Assert.That(ReferenceEquals(first, afterRegister)).IsFalse();

        registry.Insert(0, "beta", new BradixCollectionRegistryDemoItem("Beta updated"));
        IReadOnlyList<BradixCollectionEntry<BradixCollectionRegistryDemoItem>> afterInsert = registry.Snapshot();
        await Assert.That(ReferenceEquals(afterRegister, afterInsert)).IsFalse();

        registry.SetBefore("beta", "amber", new BradixCollectionRegistryDemoItem("Amber"));
        IReadOnlyList<BradixCollectionEntry<BradixCollectionRegistryDemoItem>> afterSetBefore = registry.Snapshot();
        await Assert.That(ReferenceEquals(afterInsert, afterSetBefore)).IsFalse();

        registry.SetAfter("beta", "blue", new BradixCollectionRegistryDemoItem("Blue"));
        IReadOnlyList<BradixCollectionEntry<BradixCollectionRegistryDemoItem>> afterSetAfter = registry.Snapshot();
        await Assert.That(ReferenceEquals(afterSetBefore, afterSetAfter)).IsFalse();

        registry.Unregister("amber");
        IReadOnlyList<BradixCollectionEntry<BradixCollectionRegistryDemoItem>> afterUnregister = registry.Snapshot();
        await Assert.That(ReferenceEquals(afterSetAfter, afterUnregister)).IsFalse();

        registry.Clear();
        IReadOnlyList<BradixCollectionEntry<BradixCollectionRegistryDemoItem>> afterClear = registry.Snapshot();
        await Assert.That(ReferenceEquals(afterUnregister, afterClear)).IsFalse();
        await Assert.That(ReferenceEquals(afterClear, registry.Snapshot())).IsTrue();
    }

    [Test]
    public async Task Insert_repositions_existing_entry_without_duplication()
    {
        var registry = new BradixCollectionRegistry<BradixCollectionRegistryDemoItem>();

        registry.Register("alpha", new BradixCollectionRegistryDemoItem("Alpha"));
        registry.Register("beta", new BradixCollectionRegistryDemoItem("Beta"));
        registry.Register("blue", new BradixCollectionRegistryDemoItem("Blue"));
        registry.Insert(0, "blue", new BradixCollectionRegistryDemoItem("Blue"));

        BradixCollectionEntry<BradixCollectionRegistryDemoItem>[] snapshot = [.. registry.Snapshot()];

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

}
