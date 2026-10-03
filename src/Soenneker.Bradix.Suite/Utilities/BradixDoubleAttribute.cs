using System;

namespace Soenneker.Bradix;

internal struct BradixDoubleAttribute
{
    private long _bits;
    private object? _value;

    internal object Get(double value)
    {
        long bits = BitConverter.DoubleToInt64Bits(value);
        if (_value is null || _bits != bits)
        {
            _bits = bits;
            _value = value;
        }
        return _value;
    }
}
