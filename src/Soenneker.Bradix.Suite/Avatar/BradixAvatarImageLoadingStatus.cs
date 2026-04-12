using Soenneker.Gen.EnumValues;

namespace Soenneker.Bradix;

[EnumValue<string>]
public sealed partial class BradixAvatarImageLoadingStatus
{
    public static readonly BradixAvatarImageLoadingStatus Idle = new("idle");
    public static readonly BradixAvatarImageLoadingStatus Loading = new("loading");
    public static readonly BradixAvatarImageLoadingStatus Loaded = new("loaded");
    public static readonly BradixAvatarImageLoadingStatus Error = new("error");
}
