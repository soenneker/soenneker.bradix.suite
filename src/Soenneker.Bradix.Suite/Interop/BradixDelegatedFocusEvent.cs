namespace Soenneker.Bradix;

public sealed class BradixDelegatedFocusEvent
{
    public bool DefaultPrevented { get; set; }
    public string TargetId { get; set; } = string.Empty;
    public string[] AncestorIds { get; set; } = [];
    public string RelatedTargetId { get; set; } = string.Empty;
    public string[] RelatedTargetAncestorIds { get; set; } = [];
}
