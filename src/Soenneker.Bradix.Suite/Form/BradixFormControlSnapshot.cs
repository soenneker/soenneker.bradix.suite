namespace Soenneker.Bradix;

/// <summary>
/// Represents the bradix form control snapshot.
/// </summary>
public sealed class BradixFormControlSnapshot
{
    /// <summary>
    /// Gets or sets value.
    /// </summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets validity.
    /// </summary>
    public BradixFormValiditySnapshot Validity { get; set; } = new();

    /// <summary>
    /// Gets or sets form data.
    /// </summary>
    public BradixFormDataSnapshot FormData { get; set; } = new();
}
