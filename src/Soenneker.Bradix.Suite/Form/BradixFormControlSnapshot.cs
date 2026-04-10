namespace Soenneker.Bradix.Suite.Form;

public sealed class BradixFormControlSnapshot
{
    public string Value { get; set; } = string.Empty;

    public BradixFormValiditySnapshot Validity { get; set; } = new();

    public BradixFormDataSnapshot FormData { get; set; } = new();
}
