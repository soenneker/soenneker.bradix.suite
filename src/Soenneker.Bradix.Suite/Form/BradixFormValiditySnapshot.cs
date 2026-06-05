namespace Soenneker.Bradix;

/// <summary>
/// Represents the bradix form validity snapshot.
/// </summary>
public sealed class BradixFormValiditySnapshot
{
    /// <summary>
    /// Gets or sets a value indicating whether bad input.
    /// </summary>
    public bool BadInput { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether custom error.
    /// </summary>
    public bool CustomError { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether pattern mismatch.
    /// </summary>
    public bool PatternMismatch { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether range overflow.
    /// </summary>
    public bool RangeOverflow { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether range underflow.
    /// </summary>
    public bool RangeUnderflow { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether step mismatch.
    /// </summary>
    public bool StepMismatch { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether too long.
    /// </summary>
    public bool TooLong { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether too short.
    /// </summary>
    public bool TooShort { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether type mismatch.
    /// </summary>
    public bool TypeMismatch { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether valid.
    /// </summary>
    public bool Valid { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether value missing.
    /// </summary>
    public bool ValueMissing { get; set; }

    /// <summary>
    /// Gets or sets validation message.
    /// </summary>
    public string ValidationMessage { get; set; } = string.Empty;
}
