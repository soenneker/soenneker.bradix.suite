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
    /// Executes the prevent default operation.
    /// </summary>
    public void PreventDefault()
    {
        DefaultPrevented = true;
    }
}
