using Soenneker.Bradix.Suite.Abstract;
using Soenneker.Tests.FixturedUnit;
using Xunit;

namespace Soenneker.Bradix.Suite.Tests;

[Collection("Collection")]
public sealed class BradixComponentTests : FixturedUnitTest
{
    private readonly IBradixComponent _blazorlibrary;

    public BradixComponentTests(Fixture fixture, ITestOutputHelper output) : base(fixture, output)
    {
        _blazorlibrary = Resolve<IBradixComponent>(true);
    }

    [Fact]
    public void Default()
    {

    }
}
