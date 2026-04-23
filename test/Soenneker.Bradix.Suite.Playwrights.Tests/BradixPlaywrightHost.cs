using Soenneker.Playwrights.TestHosts;
using Soenneker.Playwrights.TestEnvironment.Options;
using System.IO;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

public sealed class BradixPlaywrightHost : PlaywrightHostedTestHost
{
    protected override PlaywrightTestHostOptions CreateOptions()
    {
        return new PlaywrightTestHostOptions
        {
            SolutionFileName = "Soenneker.Bradix.Suite.slnx",
            ProjectRelativePath = Path.Combine("test", "Soenneker.Bradix.Suite.Demo", "Soenneker.Bradix.Suite.Demo.csproj"),
            ApplicationName = "Bradix demo",
            Restore = false,
            BuildConfiguration = "Debug",
            ReuseBrowserContextAcrossSessions = false,
            ReusePageAcrossSessions = false
        };
    }
}
