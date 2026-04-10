namespace Soenneker.Bradix;

public sealed class BradixDelegatedMouseEvent
{
    public long Detail { get; set; }
    public long Button { get; set; }
    public bool CtrlKey { get; set; }
    public bool ShiftKey { get; set; }
    public bool AltKey { get; set; }
    public bool MetaKey { get; set; }
    public bool DefaultPrevented { get; set; }
}
