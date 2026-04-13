using System.Diagnostics;
using Microsoft.Playwright;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

public sealed class FixtureRuntime
{
    public string BaseUrl { get; set; } = null!;

    public IPlaywright? Playwright { get; set; }

    public IBrowser? Browser { get; set; }

    public Process? DemoProcess { get; set; }
}