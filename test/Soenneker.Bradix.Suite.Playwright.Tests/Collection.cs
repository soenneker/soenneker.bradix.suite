using Xunit;

namespace Soenneker.Bradix.Suite.Playwright.Tests;

[CollectionDefinition("Collection", DisableParallelization = true)]
public sealed class Collection : ICollectionFixture<Fixture>
{
}
