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

    /// <summary>
    /// Gets or sets count.
    /// </summary>
    public int Count => _keys.Count;

    /// <summary>
    /// Gets or sets keys.
    /// </summary>
    public IEnumerable<TKey> Keys => _keys;

    /// <summary>
    /// Gets values.
    /// </summary>
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

    /// <summary>
    /// Gets or sets the element at the specified index.
    /// </summary>
    /// <param name="key">The key.</param>
    public TValue this[TKey key]
    {
        get => _map[key];
        set => Set(key, value);
    }

    /// <summary>
    /// Executes the contains key operation.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>A value indicating whether the operation succeeded.</returns>
    public bool ContainsKey(TKey key)
    {
        return _map.ContainsKey(key);
    }

    /// <summary>
    /// Attempts to get value.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    /// <returns>A value indicating whether the operation succeeded.</returns>
    public bool TryGetValue(TKey key, out TValue value)
    {
        return _map.TryGetValue(key, out value!);
    }

    /// <summary>
    /// Sets the value.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    /// <returns>The result of the operation.</returns>
    public BradixOrderedDictionary<TKey, TValue> Set(TKey key, TValue value)
    {
        if (_map.TryAdd(key, value))
            _keys.Add(key);
        else
            _map[key] = value;

        return this;
    }

    /// <summary>
    /// Executes the insert operation.
    /// </summary>
    /// <param name="index">The index.</param>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    /// <returns>The result of the operation.</returns>
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

    /// <summary>
    /// Executes the delete operation.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>A value indicating whether the operation succeeded.</returns>
    public bool Delete(TKey key)
    {
        bool removed = _map.Remove(key);

        if (removed)
        {
            _keys.Remove(key);
        }

        return removed;
    }

    /// <summary>
    /// Deletes at.
    /// </summary>
    /// <param name="index">The index.</param>
    /// <returns>A value indicating whether the operation succeeded.</returns>
    public bool DeleteAt(int index)
    {
        return TryGetKeyAt(index, out TKey key) && Delete(key);
    }

    /// <summary>
    /// Executes the clear operation.
    /// </summary>
    public void Clear()
    {
        _map.Clear();
        _keys.Clear();
    }

    /// <summary>
    /// Executes the index of operation.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>The result of the operation.</returns>
    public int IndexOf(TKey key)
    {
        return _keys.IndexOf(key);
    }

    /// <summary>
    /// Executes the key at operation.
    /// </summary>
    /// <param name="index">The index.</param>
    /// <returns>The result of the operation.</returns>
    public TKey? KeyAt(int index)
    {
        return TryGetKeyAt(index, out TKey key) ? key : default;
    }

    /// <summary>
    /// Executes the at operation.
    /// </summary>
    /// <param name="index">The index.</param>
    /// <returns>The result of the operation.</returns>
    public TValue? At(int index)
    {
        return TryGetKeyAt(index, out TKey key) ? _map[key] : default;
    }

    /// <summary>
    /// Executes the entry at operation.
    /// </summary>
    /// <param name="index">The index.</param>
    /// <returns>The result of the operation.</returns>
    public KeyValuePair<TKey, TValue>? EntryAt(int index)
    {
        return TryGetKeyAt(index, out TKey key) ? new KeyValuePair<TKey, TValue>(key, _map[key]) : null;
    }

    /// <summary>
    /// Executes the before operation.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>The result of the operation.</returns>
    public KeyValuePair<TKey, TValue>? Before(TKey key)
    {
        int index = IndexOf(key);
        return index <= 0 ? null : EntryAt(index - 1);
    }

    /// <summary>
    /// Executes the after operation.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>The result of the operation.</returns>
    public KeyValuePair<TKey, TValue>? After(TKey key)
    {
        int index = IndexOf(key);
        return index < 0 || index >= Count - 1 ? null : EntryAt(index + 1);
    }

    /// <summary>
    /// Executes the first operation.
    /// </summary>
    /// <returns>The result of the operation.</returns>
    public KeyValuePair<TKey, TValue>? First()
    {
        return EntryAt(0);
    }

    /// <summary>
    /// Executes the last operation.
    /// </summary>
    /// <returns>The result of the operation.</returns>
    public KeyValuePair<TKey, TValue>? Last()
    {
        return EntryAt(-1);
    }

    /// <summary>
    /// Sets before.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="newKey">The new key.</param>
    /// <param name="value">The value.</param>
    /// <returns>The result of the operation.</returns>
    public BradixOrderedDictionary<TKey, TValue> SetBefore(TKey key, TKey newKey, TValue value)
    {
        int index = IndexOf(key);
        return index < 0 ? this : Insert(index, newKey, value);
    }

    /// <summary>
    /// Sets after.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="newKey">The new key.</param>
    /// <param name="value">The value.</param>
    /// <returns>The result of the operation.</returns>
    public BradixOrderedDictionary<TKey, TValue> SetAfter(TKey key, TKey newKey, TValue value)
    {
        int index = IndexOf(key);
        return index < 0 ? this : Insert(index + 1, newKey, value);
    }

    /// <summary>
    /// Executes the from operation.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="offset">The offset.</param>
    /// <returns>The result of the operation.</returns>
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

    /// <summary>
    /// Executes the key from operation.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="offset">The offset.</param>
    /// <returns>The result of the operation.</returns>
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

    /// <summary>
    /// Executes the find operation.
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    /// <returns>The result of the operation.</returns>
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

    /// <summary>
    /// Executes the find index operation.
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    /// <returns>The result of the operation.</returns>
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

    /// <summary>
    /// Executes the filter operation.
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    /// <returns>The result of the operation.</returns>
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

    /// <summary>
    /// Executes the to sorted operation.
    /// </summary>
    /// <param name="comparison">The comparison.</param>
    /// <returns>The result of the operation.</returns>
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

    /// <summary>
    /// Executes the to reversed operation.
    /// </summary>
    /// <returns>The result of the operation.</returns>
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

    /// <summary>
    /// Gets enumerator.
    /// </summary>
    /// <returns>The result of the operation.</returns>
    public Enumerator GetEnumerator() => new(this);

    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() => GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <summary>
    /// Represents the enumerator structure.
    /// </summary>
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

        /// <summary>
        /// Gets current.
        /// </summary>
        public KeyValuePair<TKey, TValue> Current { get; private set; }

        readonly object IEnumerator.Current => Current;

        /// <summary>
        /// Executes the move next operation.
        /// </summary>
        /// <returns>A value indicating whether the operation succeeded.</returns>
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

        /// <summary>
        /// Executes the reset operation.
        /// </summary>
        public void Reset()
        {
            _index = -1;
            Current = default;
        }

        /// <summary>
        /// Releases resources used by the current instance.
        /// </summary>
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
