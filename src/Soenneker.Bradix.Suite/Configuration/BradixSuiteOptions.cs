namespace Soenneker.Bradix.Configuration;

/// <summary>
/// Options for configuring Bradix suite infrastructure.
/// </summary>
public sealed class BradixSuiteOptions
{
    /// <summary>
    /// Whether Bradix should load Floating UI from the CDN instead of packaged static web assets.
    /// </summary>
    public bool UseCdn { get; set; }
}
