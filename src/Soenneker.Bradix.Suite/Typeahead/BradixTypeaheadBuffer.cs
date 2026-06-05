using System;

namespace Soenneker.Bradix;

/// <summary>
/// Tracks the transient typeahead search buffer Radix uses for menu/select navigation.
/// The value expires after a short idle timeout, but also clears lazily when read.
/// </summary>
public sealed class BradixTypeaheadBuffer
{
    private readonly TimeProvider _timeProvider;
    private readonly TimeSpan _resetAfter;
    private string _search = string.Empty;
    private DateTimeOffset? _expiresAt;

    public BradixTypeaheadBuffer(TimeProvider? timeProvider = null, TimeSpan? resetAfter = null)
    {
        _timeProvider = timeProvider ?? TimeProvider.System;
        _resetAfter = resetAfter ?? TimeSpan.FromSeconds(1);
    }

    /// <summary>
    /// Gets current search.
    /// </summary>
    public string CurrentSearch
    {
        get
        {
            Refresh();
            return _search;
        }
    }

    /// <summary>
    /// Executes the append operation.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>The result of the operation.</returns>
    public string Append(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            return CurrentSearch;
        }

        Refresh();
        _search += key;
        _expiresAt = _timeProvider.GetUtcNow().Add(_resetAfter);

        return _search;
    }

    /// <summary>
    /// Executes the reset operation.
    /// </summary>
    public void Reset()
    {
        _search = string.Empty;
        _expiresAt = null;
    }

    private void Refresh()
    {
        if (_expiresAt is null)
        {
            return;
        }

        if (_timeProvider.GetUtcNow() >= _expiresAt.Value)
        {
            Reset();
        }
    }
}
