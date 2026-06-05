using Soenneker.Gen.EnumValues;

namespace Soenneker.Bradix;

/// <summary>
/// Represents the bradix avatar image loading status.
/// </summary>
[EnumValue<string>]
public sealed partial class BradixAvatarImageLoadingStatus
{
    /// <summary>
    /// The idle.
    /// </summary>
    public static readonly BradixAvatarImageLoadingStatus Idle = new("idle");
    /// <summary>
    /// The loading.
    /// </summary>
    public static readonly BradixAvatarImageLoadingStatus Loading = new("loading");
    /// <summary>
    /// The loaded.
    /// </summary>
    public static readonly BradixAvatarImageLoadingStatus Loaded = new("loaded");
    /// <summary>
    /// The error.
    /// </summary>
    public static readonly BradixAvatarImageLoadingStatus Error = new("error");
}
