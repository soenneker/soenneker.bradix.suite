namespace Soenneker.Bradix;

public class BradixPreventableEventArgs
{
    public bool DefaultPrevented { get; private set; }

    public void PreventDefault()
    {
        DefaultPrevented = true;
    }
}
