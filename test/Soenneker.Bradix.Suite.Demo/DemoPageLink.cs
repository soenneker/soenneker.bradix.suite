namespace Soenneker.Bradix.Suite.Demo;

public sealed record DemoPageLink(string Route, string Title, string Category, string Description)
{
    public string Href => Route == "/" ? string.Empty : Route.TrimStart('/');
    public string Slug => Route == "/" ? "overview" : Route.Trim('/').Replace('-', ' ');
}
