using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Soenneker.Bradix.Suite.Collection;

/// <summary>
/// Ordered key/value storage modeled after the Radix collection substrate.
/// Updating an existing key preserves its position unless explicitly reinserted.
/// </summary>
public sealed class BradixOrderedDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    where TKey : notnull
{
    private readonly Dictionary<TKey, TValue> _map = new();
    private readonly List<TKey> _keys = [];

    public BradixOrderedDictionary()
    {
    }

    public BradixOrderedDictionary(IEnumerable<KeyValuePair<TKey, TValue>> entries)
    {
        foreach (KeyValuePair<TKey, TValue> entry in entries)
        {
            Set(entry.Key, entry.Value);
        }
    }

    public int Count => _keys.Count;

    public IEnumerable<TKey> Keys => _keys.ToArray();

    public IEnumerable<TValue> Values => _keys.Select(key => _map[key]);

    public TValue this[TKey key]
    {
        get => _map[key];
        set => Set(key, value);
    }

    public bool ContainsKey(TKey key)
    {
        return _map.ContainsKey(key);
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        return _map.TryGetValue(key, out value!);
    }

    public BradixOrderedDictionary<TKey, TValue> Set(TKey key, TValue value)
    {
        if (!_map.ContainsKey(key))
        {
            _keys.Add(key);
        }

        _map[key] = value;
        return this;
    }

    public BradixOrderedDictionary<TKey, TValue> Insert(int index, TKey key, TValue value)
    {
        bool exists = _map.ContainsKey(key);

        if (exists)
        {
            int existingIndex = _keys.IndexOf(key);
            if (existingIndex >= 0)
            {
                _keys.RemoveAt(existingIndex);

                if (existingIndex < index)
                {
                    index--;
                }
            }
        }

        int targetIndex = NormalizeInsertIndex(index, _keys.Count);
        _keys.Insert(targetIndex, key);
        _map[key] = value;

        return this;
    }

    public bool Delete(TKey key)
    {
        bool removed = _map.Remove(key);

        if (removed)
        {
            _keys.Remove(key);
        }

        return removed;
    }

    public void Clear()
    {
        _map.Clear();
        _keys.Clear();
    }

    public int IndexOf(TKey key)
    {
        return _keys.IndexOf(key);
    }

    public TKey? KeyAt(int index)
    {
        int safeIndex = NormalizeLookupIndex(index, _keys.Count);
        return safeIndex < 0 ? default : _keys[safeIndex];
    }

    public TValue? At(int index)
    {
        TKey? key = KeyAt(index);
        return key is null ? default : _map[key];
    }

    public KeyValuePair<TKey, TValue>? EntryAt(int index)
    {
        TKey? key = KeyAt(index);
        return key is null ? null : new KeyValuePair<TKey, TValue>(key, _map[key]);
    }

    public KeyValuePair<TKey, TValue>? Before(TKey key)
    {
        return EntryAt(IndexOf(key) - 1);
    }

    public KeyValuePair<TKey, TValue>? After(TKey key)
    {
        return EntryAt(IndexOf(key) + 1);
    }

    public TValue? From(TKey key, int offset)
    {
        int index = IndexOf(key);
        if (index < 0)
        {
            return default;
        }

        int destination = Math.Clamp(index + offset, 0, Count - 1);
        return At(destination);
    }

    public BradixOrderedDictionary<TKey, TValue> ToSorted(Comparison<KeyValuePair<TKey, TValue>> comparison)
    {
        List<KeyValuePair<TKey, TValue>> entries = [.. this];
        entries.Sort(comparison);
        return new BradixOrderedDictionary<TKey, TValue>(entries);
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        foreach (TKey key in _keys)
        {
            yield return new KeyValuePair<TKey, TValue>(key, _map[key]);
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private static int NormalizeInsertIndex(int index, int count)
    {
        if (count == 0)
        {
            return 0;
        }

        int normalized = index >= 0 ? index : count + index + 1;
        return Math.Clamp(normalized, 0, count);
    }

    private static int NormalizeLookupIndex(int index, int count)
    {
        if (count == 0)
        {
            return -1;
        }

        int normalized = index >= 0 ? index : count + index;
        return normalized < 0 || normalized >= count ? -1 : normalized;
    }
}
