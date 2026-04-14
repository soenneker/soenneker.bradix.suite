using Soenneker.Playwrights.Fixtures;
using Soenneker.Playwrights.TestEnvironment;
using System.IO;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

public class BradixPlaywrightFixture : PlaywrightFixture
{
    protected override PlaywrightFixtureOptions CreateOptions()
    {
        return new PlaywrightFixtureOptions
        {
            SolutionFileName = "Soenneker.Bradix.Suite.slnx",
            ProjectRelativePath = Path.Combine("test", "Soenneker.Bradix.Suite.Demo", "Soenneker.Bradix.Suite.Demo.csproj"),
            ApplicationName = "Bradix demo"
        };
    }
}