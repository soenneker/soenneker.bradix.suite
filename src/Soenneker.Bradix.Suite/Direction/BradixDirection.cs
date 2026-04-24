namespace Soenneker.Bradix;

internal static class BradixDirection
{
    internal const string LeftToRight = "ltr";
    internal const string RightToLeft = "rtl";

    internal static string Normalize(string? dir)
    {
        return dir is RightToLeft ? RightToLeft : LeftToRight;
    }

    internal static string Resolve(string? localDir, string? cascadedDir)
    {
        return localDir is LeftToRight or RightToLeft
            ? localDir
            : Normalize(cascadedDir);
    }
}
