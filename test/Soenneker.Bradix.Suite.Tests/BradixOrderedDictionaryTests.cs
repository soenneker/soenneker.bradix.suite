using System.Linq;
using System.Threading.Tasks;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixOrderedDictionaryTests
{
    [Test]
    public async Task Set_preserves_existing_key_position()
    {
        var dictionary = new BradixOrderedDictionary<string, int>();

        dictionary.Set("alpha", 1);
        dictionary.Set("beta", 2);
        dictionary.Set("alpha", 3);

        await Assert.That(string.Join(",", dictionary.Keys)).IsEqualTo("alpha,beta");
        await Assert.That(dictionary["alpha"]).IsEqualTo(3);
    }

    [Test]
    public async Task Insert_moves_existing_key_to_requested_position()
    {
        var dictionary = new BradixOrderedDictionary<string, int>();

        dictionary.Set("alpha", 1);
        dictionary.Set("beta", 2);
        dictionary.Set("blue", 3);
        dictionary.Insert(0, "blue", 30);

        await Assert.That(string.Join(",", dictionary.Keys)).IsEqualTo("blue,alpha,beta");
        await Assert.That(dictionary["blue"]).IsEqualTo(30);
    }

    [Test]
    public async Task Before_after_and_from_follow_current_order()
    {
        var dictionary = new BradixOrderedDictionary<string, int>();

        dictionary.Set("alpha", 1);
        dictionary.Set("beta", 2);
        dictionary.Set("gamma", 3);

        await Assert.That(dictionary.Before("beta")?.Key).IsEqualTo("alpha");
        await Assert.That(dictionary.After("beta")?.Key).IsEqualTo("gamma");
        await Assert.That(dictionary.From("alpha", 2)).IsEqualTo(3);
        await Assert.That(dictionary.From("beta", -1)).IsEqualTo(1);
    }
}