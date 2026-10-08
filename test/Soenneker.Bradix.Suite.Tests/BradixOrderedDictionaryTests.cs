using System.Threading.Tasks;
using System.Threading;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixOrderedDictionaryTests
{
    [Test]
    public async ValueTask DeleteAt_preserves_order_and_supports_negative_indices(CancellationToken cancellationToken)
    {
        var dictionary = new BradixOrderedDictionary<string, int>();
        dictionary.Set("alpha", 1).Set("beta", 2).Set("gamma", 3).Set("delta", 4);

        await Assert.That(dictionary.DeleteAt(1)).IsTrue();
        await Assert.That(dictionary.ContainsKey("beta")).IsFalse();
        await Assert.That(string.Join(",", dictionary.Keys)).IsEqualTo("alpha,gamma,delta");
        await Assert.That(dictionary.DeleteAt(-1)).IsTrue();
        await Assert.That(dictionary.DeleteAt(-2)).IsTrue();
        await Assert.That(dictionary.Count).IsEqualTo(1);
        await Assert.That(dictionary.At(0)).IsEqualTo(3);
        await Assert.That(dictionary.DeleteAt(-2)).IsFalse();
        await Assert.That(dictionary.DeleteAt(1)).IsFalse();
        await Assert.That(dictionary.DeleteAt(0)).IsTrue();
        await Assert.That(dictionary.DeleteAt(0)).IsFalse();
    }

    [Test]
    public async ValueTask Set_preserves_existing_key_position(CancellationToken cancellationToken)
    {
        var dictionary = new BradixOrderedDictionary<string, int>();

        dictionary.Set("alpha", 1);
        dictionary.Set("beta", 2);
        dictionary.Set("alpha", 3);

        await Assert.That(string.Join(",", dictionary.Keys)).IsEqualTo("alpha,beta");
        await Assert.That(dictionary["alpha"]).IsEqualTo(3);
    }

    [Test]
    public async ValueTask Insert_moves_existing_key_to_requested_position(CancellationToken cancellationToken)
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
    public async ValueTask Before_after_and_from_follow_current_order(CancellationToken cancellationToken)
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

    [Test]
    public async ValueTask Before_and_after_return_null_at_boundaries_and_for_missing_keys(CancellationToken cancellationToken)
    {
        var dictionary = new BradixOrderedDictionary<string, int>();

        dictionary.Set("alpha", 1);
        dictionary.Set("beta", 2);
        dictionary.Set("gamma", 3);

        await Assert.That(dictionary.Before("alpha")).IsNull();
        await Assert.That(dictionary.After("gamma")).IsNull();
        await Assert.That(dictionary.Before("missing")).IsNull();
        await Assert.That(dictionary.After("missing")).IsNull();
    }
}
