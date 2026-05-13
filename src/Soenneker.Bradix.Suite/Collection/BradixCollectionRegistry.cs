using System.Collections.Generic;
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
        var snapshot = new BradixCollectionEntry<TItem>[_items.Count];
        var index = 0;

        foreach (KeyValuePair<string, TItem> entry in _items)
        {
            snapshot[index++] = new BradixCollectionEntry<TItem>(entry.Key, entry.Value);
        }

        return snapshot;
    }

    public BradixOrderedDictionary<string, TItem>.Enumerator GetEnumerator()
    {
        return _items.GetEnumerator();
    }

    public void Clear()
    {
        _items.Clear();
    }
}
