using System;
using System.Collections;
using System.Collections.Generic;

namespace Soenneker.Bradix;

/// <summary>
/// Ordered key/value storage modeled after the Radix collection substrate.
/// Updating an existing key preserves its position unless explicitly reinserted.
/// </summary>
public sealed class BradixOrderedDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    where TKey : notnull
{
    private readonly Dictionary<TKey, TValue> _map;
    private readonly List<TKey> _keys;

    public BradixOrderedDictionary()
    {
        _map = new Dictionary<TKey, TValue>();
        _keys = [];
    }

    public BradixOrderedDictionary(IEnumerable<KeyValuePair<TKey, TValue>> entries)
    {
        var capacity = entries is ICollection<KeyValuePair<TKey, TValue>> collection ? collection.Count : 0;

        _map = capacity > 0 ? new Dictionary<TKey, TValue>(capacity) : new Dictionary<TKey, TValue>();
        _keys = capacity > 0 ? new List<TKey>(capacity) : [];

        foreach (KeyValuePair<TKey, TValue> entry in entries)
        {
            Set(entry.Key, entry.Value);
        }
    }

    private BradixOrderedDictionary(int capacity)
    {
        _map = capacity > 0 ? new Dictionary<TKey, TValue>(capacity) : new Dictionary<TKey, TValue>();
        _keys = capacity > 0 ? new List<TKey>(capacity) : [];
    }

    public int Count => _keys.Count;

    public IEnumerable<TKey> Keys => _keys;

    public IEnumerable<TValue> Values
    {
        get
        {
            foreach (TKey key in _keys)
            {
                yield return _map[key];
            }
        }
    }

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
        if (_map.TryAdd(key, value))
            _keys.Add(key);
        else
            _map[key] = value;

        return this;
    }

    public BradixOrderedDictionary<TKey, TValue> Insert(int index, TKey key, TValue value)
    {
        bool has = _map.ContainsKey(key);
        int length = _keys.Count;
        int actualIndex = index >= 0 ? index : length + index;
        int safeIndex = actualIndex < 0 || actualIndex >= length ? -1 : actualIndex;

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
            int existingIndex = _keys.IndexOf(key);
            if (existingIndex >= 0)
            {
                _keys.RemoveAt(existingIndex);

                if (existingIndex < actualIndex)
                {
                    actualIndex--;
                }
            }
        }

        int targetIndex = Math.Clamp(actualIndex, 0, _keys.Count);
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
        int index = IndexOf(key);
        return index < 0 ? this : Insert(index, newKey, value);
    }

    public BradixOrderedDictionary<TKey, TValue> SetAfter(TKey key, TKey newKey, TValue value)
    {
        int index = IndexOf(key);
        return index < 0 ? this : Insert(index + 1, newKey, value);
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

    public TKey? KeyFrom(TKey key, int offset)
    {
        int index = IndexOf(key);
        if (index < 0)
        {
            return default;
        }

        int destination = Math.Clamp(index + offset, 0, Count - 1);
        return KeyAt(destination);
    }

    public KeyValuePair<TKey, TValue>? Find(Predicate<KeyValuePair<TKey, TValue>> predicate)
    {
        foreach (KeyValuePair<TKey, TValue> entry in this)
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

        foreach (KeyValuePair<TKey, TValue> entry in this)
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
        var filtered = new BradixOrderedDictionary<TKey, TValue>(_keys.Count);

        foreach (KeyValuePair<TKey, TValue> entry in this)
        {
            if (predicate(entry))
                filtered.Set(entry.Key, entry.Value);
        }

        return filtered;
    }

    public BradixOrderedDictionary<TKey, TValue> ToSorted(Comparison<KeyValuePair<TKey, TValue>> comparison)
    {
        var entries = new List<KeyValuePair<TKey, TValue>>(_keys.Count);

        foreach (KeyValuePair<TKey, TValue> entry in this)
        {
            entries.Add(entry);
        }

        entries.Sort(comparison);
        return new BradixOrderedDictionary<TKey, TValue>(entries);
    }

    public BradixOrderedDictionary<TKey, TValue> ToReversed()
    {
        var reversed = new BradixOrderedDictionary<TKey, TValue>(_keys.Count);

        for (int i = _keys.Count - 1; i >= 0; i--)
        {
            TKey key = _keys[i];
            reversed.Set(key, _map[key]);
        }

        return reversed;
    }

    public Enumerator GetEnumerator() => new(this);

    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() => GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>
    {
        private readonly BradixOrderedDictionary<TKey, TValue> _dictionary;
        private int _index;

        internal Enumerator(BradixOrderedDictionary<TKey, TValue> dictionary)
        {
            _dictionary = dictionary;
            _index = -1;
            Current = default;
        }

        public KeyValuePair<TKey, TValue> Current { get; private set; }

        readonly object IEnumerator.Current => Current;

        public bool MoveNext()
        {
            int nextIndex = _index + 1;

            if (nextIndex >= _dictionary._keys.Count)
            {
                _index = _dictionary._keys.Count;
                Current = default;
                return false;
            }

            _index = nextIndex;
            TKey key = _dictionary._keys[nextIndex];
            Current = new KeyValuePair<TKey, TValue>(key, _dictionary._map[key]);
            return true;
        }

        public void Reset()
        {
            _index = -1;
            Current = default;
        }

        public readonly void Dispose()
        {
        }
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

    private bool TryGetKeyAt(int index, out TKey key)
    {
        int safeIndex = NormalizeLookupIndex(index, _keys.Count);
        if (safeIndex < 0)
        {
            key = default!;
            return false;
        }

        key = _keys[safeIndex];
        return true;
    }
}
