namespace Soenneker.Bradix;

public sealed class BradixDelegatedKeyboardEvent
{
    public string Key { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public bool CtrlKey { get; set; }
    public bool ShiftKey { get; set; }
    public bool AltKey { get; set; }
    public bool MetaKey { get; set; }
    public bool Repeat { get; set; }
    public bool DefaultPrevented { get; set; }
}
