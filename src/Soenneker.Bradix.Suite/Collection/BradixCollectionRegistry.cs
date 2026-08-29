using System;
using System.Collections.Generic;

namespace Soenneker.Bradix;

/// <summary>
/// Reusable ordered registry for menu/select style item collections.
/// </summary>
public sealed class BradixCollectionRegistry<TItem>
{
    private readonly BradixOrderedDictionary<string, TItem> _items = new();
    private IReadOnlyList<BradixCollectionEntry<TItem>>? _snapshot;

    /// <summary>
    /// Gets or sets count.
    /// </summary>
    public int Count => _items.Count;

    /// <summary>
    /// Registers a callback with the Bradix Collection Registry.
    /// </summary>
    /// <param name="key">Key used to locate the target entry.</param>
    /// <param name="item">Receives the entry when the key is found.</param>
    public void Register(string key, TItem item)
    {
        _items.Set(key, item);
        _snapshot = null;
    }

    /// <summary>
    /// Inserts bradix Collection Registry for the Bradix Collection Registry.
    /// </summary>
    /// <param name="index">Zero-based position of the target item.</param>
    /// <param name="key">Key used to locate the target entry.</param>
    /// <param name="item">Receives the entry when the key is found.</param>
    public void Insert(int index, string key, TItem item)
    {
        _items.Insert(index, key, item);
        _snapshot = null;
    }

    /// <summary>
    /// Sets before.
    /// </summary>
    /// <param name="key">Key used to locate the target entry.</param>
    /// <param name="newKey">Replacement key to insert.</param>
    /// <param name="item">Receives the entry when the key is found.</param>
    public void SetBefore(string key, string newKey, TItem item)
    {
        if (!_items.ContainsKey(key))
            return;

        _items.SetBefore(key, newKey, item);
        _snapshot = null;
    }

    /// <summary>
    /// Sets after.
    /// </summary>
    /// <param name="key">Key used to locate the target entry.</param>
    /// <param name="newKey">Replacement key to insert.</param>
    /// <param name="item">Receives the entry when the key is found.</param>
    public void SetAfter(string key, string newKey, TItem item)
    {
        if (!_items.ContainsKey(key))
            return;

        _items.SetAfter(key, newKey, item);
        _snapshot = null;
    }

    /// <summary>
    /// Removes the callback identified by the supplied ID from the Bradix Collection Registry.
    /// </summary>
    /// <param name="key">Key used to locate the target entry.</param>
    /// <returns>true if removes the callback identified by the supplied ID from the Bradix Collection Registry; otherwise, false.</returns>
    public bool Unregister(string key)
    {
        bool removed = _items.Delete(key);

        if (removed)
            _snapshot = null;

        return removed;
    }

    /// <summary>
    /// Attempts to retrieve the entry for the specified key without creating a new value.
    /// </summary>
    /// <param name="key">Key used to locate the target entry.</param>
    /// <param name="item">Receives the entry when the key is found.</param>
    /// <returns>true if a matching value was found and assigned to the output parameter; otherwise, false.</returns>
    public bool TryGet(string key, out TItem item)
    {
        return _items.TryGetValue(key, out item!);
    }

    /// <summary>
    /// Returns the value produced by snapshot.
    /// </summary>
    /// <returns>The requested collection.</returns>
    public IReadOnlyList<BradixCollectionEntry<TItem>> Snapshot()
    {
        if (_snapshot is not null)
            return _snapshot;

        var snapshot = new BradixCollectionEntry<TItem>[_items.Count];
        var index = 0;

        foreach (KeyValuePair<string, TItem> entry in _items)
        {
            snapshot[index++] = new BradixCollectionEntry<TItem>(entry.Key, entry.Value);
        }

        _snapshot = Array.AsReadOnly(snapshot);
        return _snapshot;
    }

    /// <summary>
    /// Gets enumerator.
    /// </summary>
    /// <returns>The requested bradix Ordered Dictionary.Enumerator.</returns>
    public BradixOrderedDictionary<string, TItem>.Enumerator GetEnumerator()
    {
        return _items.GetEnumerator();
    }

    /// <summary>
    /// Removes all entries managed by the Bradix Collection Registry.
    /// </summary>
    public void Clear()
    {
        if (_items.Count == 0)
            return;

        _items.Clear();
        _snapshot = null;
    }
}
