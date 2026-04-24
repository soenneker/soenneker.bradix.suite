using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Soenneker.Bradix;

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
        foreach (var entry in entries)
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
        var has = _map.ContainsKey(key);
        var length = _keys.Count;
        var actualIndex = index >= 0 ? index : length + index;
        var safeIndex = actualIndex < 0 || actualIndex >= length ? -1 : actualIndex;

        if (safeIndex == -1 || (has && safeIndex == Count - 1))
        {
            Set(key, value);
            return this;
        }

        if (index < 0)
        {
            actualIndex++;
        }

        if (has)
        {
            var existingIndex = _keys.IndexOf(key);
            if (existingIndex >= 0)
            {
                _keys.RemoveAt(existingIndex);

                if (existingIndex < actualIndex)
                {
                    actualIndex--;
                }
            }
        }

        var targetIndex = Math.Clamp(actualIndex, 0, _keys.Count);
        _keys.Insert(targetIndex, key);
        _map[key] = value;

        return this;
    }

    public bool Delete(TKey key)
    {
        var removed = _map.Remove(key);

        if (removed)
        {
            _keys.Remove(key);
        }

        return removed;
    }

    public bool DeleteAt(int index)
    {
        return TryGetKeyAt(index, out TKey key) && Delete(key);
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
        return TryGetKeyAt(index, out TKey key) ? key : default;
    }

    public TValue? At(int index)
    {
        return TryGetKeyAt(index, out TKey key) ? _map[key] : default;
    }

    public KeyValuePair<TKey, TValue>? EntryAt(int index)
    {
        return TryGetKeyAt(index, out TKey key) ? new KeyValuePair<TKey, TValue>(key, _map[key]) : null;
    }

    public KeyValuePair<TKey, TValue>? Before(TKey key)
    {
        return EntryAt(IndexOf(key) - 1);
    }

    public KeyValuePair<TKey, TValue>? After(TKey key)
    {
        return EntryAt(IndexOf(key) + 1);
    }

    public KeyValuePair<TKey, TValue>? First()
    {
        return EntryAt(0);
    }

    public KeyValuePair<TKey, TValue>? Last()
    {
        return EntryAt(-1);
    }

    public BradixOrderedDictionary<TKey, TValue> SetBefore(TKey key, TKey newKey, TValue value)
    {
        var index = IndexOf(key);
        return index < 0 ? this : Insert(index, newKey, value);
    }

    public BradixOrderedDictionary<TKey, TValue> SetAfter(TKey key, TKey newKey, TValue value)
    {
        var index = IndexOf(key);
        return index < 0 ? this : Insert(index + 1, newKey, value);
    }

    public TValue? From(TKey key, int offset)
    {
        var index = IndexOf(key);
        if (index < 0)
        {
            return default;
        }

        var destination = Math.Clamp(index + offset, 0, Count - 1);
        return At(destination);
    }

    public TKey? KeyFrom(TKey key, int offset)
    {
        var index = IndexOf(key);
        if (index < 0)
        {
            return default;
        }

        var destination = Math.Clamp(index + offset, 0, Count - 1);
        return KeyAt(destination);
    }

    public KeyValuePair<TKey, TValue>? Find(Predicate<KeyValuePair<TKey, TValue>> predicate)
    {
        foreach (var entry in this)
        {
            if (predicate(entry))
            {
                return entry;
            }
        }

        return null;
    }

    public int FindIndex(Predicate<KeyValuePair<TKey, TValue>> predicate)
    {
        var index = 0;

        foreach (var entry in this)
        {
            if (predicate(entry))
            {
                return index;
            }

            index++;
        }

        return -1;
    }

    public BradixOrderedDictionary<TKey, TValue> Filter(Predicate<KeyValuePair<TKey, TValue>> predicate)
    {
        return new BradixOrderedDictionary<TKey, TValue>(this.Where(entry => predicate(entry)));
    }

    public BradixOrderedDictionary<TKey, TValue> ToSorted(Comparison<KeyValuePair<TKey, TValue>> comparison)
    {
        List<KeyValuePair<TKey, TValue>> entries = [.. this];
        entries.Sort(comparison);
        return new BradixOrderedDictionary<TKey, TValue>(entries);
    }

    public BradixOrderedDictionary<TKey, TValue> ToReversed()
    {
        var reversed = new BradixOrderedDictionary<TKey, TValue>();

        for (var i = _keys.Count - 1; i >= 0; i--)
        {
            TKey key = _keys[i];
            reversed.Set(key, _map[key]);
        }

        return reversed;
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        foreach (var key in _keys)
        {
            yield return new KeyValuePair<TKey, TValue>(key, _map[key]);
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private static int NormalizeLookupIndex(int index, int count)
    {
        if (count == 0)
        {
            return -1;
        }

        var normalized = index >= 0 ? index : count + index;
        return normalized < 0 || normalized >= count ? -1 : normalized;
    }

    private bool TryGetKeyAt(int index, out TKey key)
    {
        var safeIndex = NormalizeLookupIndex(index, _keys.Count);
        if (safeIndex < 0)
        {
            key = default!;
            return false;
        }

        key = _keys[safeIndex];
        return true;
    }
}
