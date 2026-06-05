using System.Collections.Generic;
namespace Soenneker.Bradix;

/// <summary>
/// Reusable ordered registry for menu/select style item collections.
/// </summary>
public sealed class BradixCollectionRegistry<TItem>
{
    private readonly BradixOrderedDictionary<string, TItem> _items = new();

    /// <summary>
    /// Gets or sets count.
    /// </summary>
    public int Count => _items.Count;

    /// <summary>
    /// Executes the register operation.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="item">The item.</param>
    public void Register(string key, TItem item)
    {
        _items.Set(key, item);
    }

    /// <summary>
    /// Executes the insert operation.
    /// </summary>
    /// <param name="index">The index.</param>
    /// <param name="key">The key.</param>
    /// <param name="item">The item.</param>
    public void Insert(int index, string key, TItem item)
    {
        _items.Insert(index, key, item);
    }

    /// <summary>
    /// Sets before.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="newKey">The new key.</param>
    /// <param name="item">The item.</param>
    public void SetBefore(string key, string newKey, TItem item)
    {
        _items.SetBefore(key, newKey, item);
    }

    /// <summary>
    /// Sets after.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="newKey">The new key.</param>
    /// <param name="item">The item.</param>
    public void SetAfter(string key, string newKey, TItem item)
    {
        _items.SetAfter(key, newKey, item);
    }

    /// <summary>
    /// Executes the unregister operation.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>A value indicating whether the operation succeeded.</returns>
    public bool Unregister(string key)
    {
        return _items.Delete(key);
    }

    /// <summary>
    /// Attempts to execute get.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="item">The item.</param>
    /// <returns>A value indicating whether the operation succeeded.</returns>
    public bool TryGet(string key, out TItem item)
    {
        return _items.TryGetValue(key, out item!);
    }

    /// <summary>
    /// Executes the snapshot operation.
    /// </summary>
    /// <returns>The result of the operation.</returns>
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

    /// <summary>
    /// Gets enumerator.
    /// </summary>
    /// <returns>The result of the operation.</returns>
    public BradixOrderedDictionary<string, TItem>.Enumerator GetEnumerator()
    {
        return _items.GetEnumerator();
    }

    /// <summary>
    /// Executes the clear operation.
    /// </summary>
    public void Clear()
    {
        _items.Clear();
    }
}
