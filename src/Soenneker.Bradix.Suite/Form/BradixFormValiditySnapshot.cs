namespace Soenneker.Bradix;

public sealed class BradixFormValiditySnapshot
{
    public bool BadInput { get; set; }

    public bool CustomError { get; set; }

    public bool PatternMismatch { get; set; }

    public bool RangeOverflow { get; set; }

    public bool RangeUnderflow { get; set; }

    public bool StepMismatch { get; set; }

    public bool TooLong { get; set; }

    public bool TooShort { get; set; }

    public bool TypeMismatch { get; set; }

    public bool Valid { get; set; } = true;

    public bool ValueMissing { get; set; }

    public string ValidationMessage { get; set; } = string.Empty;
}
