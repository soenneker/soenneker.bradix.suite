using System;
using System.Globalization;

namespace Soenneker.Bradix;

internal struct BradixInvariantDoubleTextCache
{
    private long _bits;
    private string? _text;

    internal string Get(double value)
    {
        long bits = BitConverter.DoubleToInt64Bits(value);
        if (_text is null || _bits != bits)
        {
            _bits = bits;
            _text = value.ToString(CultureInfo.InvariantCulture);
        }
        return _text;
    }
}
