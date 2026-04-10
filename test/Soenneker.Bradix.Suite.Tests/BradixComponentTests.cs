using Soenneker.Bradix.Suite.Abstract;
using Soenneker.Tests.FixturedUnit;
using Xunit;

namespace Soenneker.Bradix.Suite.Tests;

[Collection("Collection")]
public sealed class BradixComponentTests : FixturedUnitTest
{
    private readonly IBradixComponent _bradixComponent;

    public BradixComponentTests(Fixture fixture, ITestOutputHelper output) : base(fixture, output)
    {
        _bradixComponent = Resolve<IBradixComponent>(true);
    }

    [Fact]
    public void Resolves()
    {
        Assert.NotNull(_bradixComponent);
    }
}
