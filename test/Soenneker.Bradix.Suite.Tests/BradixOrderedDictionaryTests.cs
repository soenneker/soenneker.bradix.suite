using Soenneker.Bradix.Suite.Collection;
using System.Linq;
using Xunit;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixOrderedDictionaryTests
{
    [Fact]
    public void Set_preserves_existing_key_position()
    {
        var dictionary = new BradixOrderedDictionary<string, int>();

        dictionary.Set("alpha", 1);
        dictionary.Set("beta", 2);
        dictionary.Set("alpha", 3);

        Assert.Equal(["alpha", "beta"], dictionary.Keys.ToArray());
        Assert.Equal(3, dictionary["alpha"]);
    }

    [Fact]
    public void Insert_moves_existing_key_to_requested_position()
    {
        var dictionary = new BradixOrderedDictionary<string, int>();

        dictionary.Set("alpha", 1);
        dictionary.Set("beta", 2);
        dictionary.Set("blue", 3);
        dictionary.Insert(0, "blue", 30);

        Assert.Equal(["blue", "alpha", "beta"], dictionary.Keys.ToArray());
        Assert.Equal(30, dictionary["blue"]);
    }

    [Fact]
    public void Before_after_and_from_follow_current_order()
    {
        var dictionary = new BradixOrderedDictionary<string, int>();

        dictionary.Set("alpha", 1);
        dictionary.Set("beta", 2);
        dictionary.Set("gamma", 3);

        Assert.Equal("alpha", dictionary.Before("beta")?.Key);
        Assert.Equal("gamma", dictionary.After("beta")?.Key);
        Assert.Equal(3, dictionary.From("alpha", 2));
        Assert.Equal(1, dictionary.From("beta", -1));
    }
}
