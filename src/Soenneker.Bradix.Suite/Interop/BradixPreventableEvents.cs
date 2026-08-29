namespace Soenneker.Bradix;

/// <summary>
/// Represents the bradix preventable event args.
/// </summary>
public class BradixPreventableEventArgs
{
    /// <summary>
    /// Gets a value indicating whether default prevented.
    /// </summary>
    public bool DefaultPrevented { get; private set; }

    /// <summary>
    /// Prevent Default on the Bradix Preventable Event Args.
    /// </summary>
    public void PreventDefault()
    {
        DefaultPrevented = true;
    }
}
