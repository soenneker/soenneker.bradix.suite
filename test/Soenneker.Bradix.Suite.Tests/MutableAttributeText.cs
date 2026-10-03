namespace Soenneker.Bradix.Suite.Tests;

internal sealed class MutableAttributeText(string value)
{
    public string Value { get; set; } = value;
    public override string ToString() => Value;
}
