using System.Collections.Generic;
using System.Linq;

namespace Soenneker.Bradix;

/// <summary>
/// Reusable ordered registry for menu/select style item collections.
/// </summary>
public sealed class BradixCollectionRegistry<TItem>
{
    private readonly BradixOrderedDictionary<string, TItem> _items = new();

    public int Count => _items.Count;

    public void Register(string key, TItem item)
    {
        _items.Set(key, item);
    }

    public void Insert(int index, string key, TItem item)
    {
        _items.Insert(index, key, item);
    }

    public void SetBefore(string key, string newKey, TItem item)
    {
        _items.SetBefore(key, newKey, item);
    }

    public void SetAfter(string key, string newKey, TItem item)
    {
        _items.SetAfter(key, newKey, item);
    }

    public bool Unregister(string key)
    {
        return _items.Delete(key);
    }

    public bool TryGet(string key, out TItem item)
    {
        return _items.TryGetValue(key, out item!);
    }

    public IReadOnlyList<BradixCollectionEntry<TItem>> Snapshot()
    {
        return _items.Select(entry => new BradixCollectionEntry<TItem>(entry.Key, entry.Value)).ToArray();
    }

    public void Clear()
    {
        _items.Clear();
    }
}

public sealed record BradixCollectionEntry<TItem>(string Key, TItem Item);
