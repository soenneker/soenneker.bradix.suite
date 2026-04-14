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

    private sealed record DemoItem(string TextValue);
}