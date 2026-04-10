namespace Soenneker.Bradix.Suite.Select;

internal sealed class BradixSelectGroupContext
{
    public string? LabelId { get; private set; }

    public void RegisterLabelId(string id)
    {
        LabelId = id;
    }
}
