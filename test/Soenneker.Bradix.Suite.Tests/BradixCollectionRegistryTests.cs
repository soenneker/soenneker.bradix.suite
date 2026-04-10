using System.Linq;
using Xunit;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixCollectionRegistryTests
{
    [Fact]
    public void Snapshot_returns_keys_in_registration_order()
    {
        var registry = new BradixCollectionRegistry<DemoItem>();

        registry.Register("alpha", new DemoItem("Alpha"));
        registry.Register("beta", new DemoItem("Beta"));
        registry.Register("blue", new DemoItem("Blue"));

        Assert.Equal(["alpha", "beta", "blue"], registry.Snapshot().Select(entry => entry.Key).ToArray());
    }

    [Fact]
    public void Insert_repositions_existing_entry_without_duplication()
    {
        var registry = new BradixCollectionRegistry<DemoItem>();

        registry.Register("alpha", new DemoItem("Alpha"));
        registry.Register("beta", new DemoItem("Beta"));
        registry.Register("blue", new DemoItem("Blue"));
        registry.Insert(0, "blue", new DemoItem("Blue"));

        BradixCollectionEntry<DemoItem>[] snapshot = [.. registry.Snapshot()];

        Assert.Equal(["blue", "alpha", "beta"], snapshot.Select(entry => entry.Key).ToArray());
        Assert.Single(snapshot, entry => entry.Key == "blue");
    }

    private sealed record DemoItem(string TextValue);
}
