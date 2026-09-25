namespace Soenneker.Bradix;

/// <summary>Options passed to the popper JavaScript module.</summary>
public sealed class BradixPopperInteropOptions
{
    /// <summary>Gets or sets the placement side.</summary>
    public string Side { get; set; } = "bottom";

    /// <summary>Gets or sets the side offset.</summary>
    public double SideOffset { get; set; }

    /// <summary>Gets or sets the placement alignment.</summary>
    public string Align { get; set; } = "center";

    /// <summary>Gets or sets the alignment offset.</summary>
    public double AlignOffset { get; set; }

    /// <summary>Gets or sets the arrow padding.</summary>
    public double ArrowPadding { get; set; }

    /// <summary>Gets or sets whether placement avoids collisions.</summary>
    public bool AvoidCollisions { get; set; } = true;

    /// <summary>Gets or sets the collision padding.</summary>
    public double CollisionPadding { get; set; }

    /// <summary>Gets or sets the collision boundary selectors.</summary>
    public string[] CollisionBoundarySelectors { get; set; } = [];

    /// <summary>Gets or sets the sticky behavior.</summary>
    public string Sticky { get; set; } = "partial";

    /// <summary>Gets or sets whether detached content is hidden.</summary>
    public bool HideWhenDetached { get; set; }

    /// <summary>Gets or sets the text direction.</summary>
    public string Dir { get; set; } = "ltr";
}
