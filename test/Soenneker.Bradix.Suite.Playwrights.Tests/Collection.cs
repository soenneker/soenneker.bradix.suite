using Xunit;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

[CollectionDefinition("Collection", DisableParallelization = true)]
public sealed class Collection : ICollectionFixture<BradixPlaywrightFixture>
{
}
