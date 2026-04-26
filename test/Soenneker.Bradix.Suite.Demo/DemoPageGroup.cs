using System.Collections.Generic;

namespace Soenneker.Bradix.Suite.Demo;

public sealed record DemoPageGroup(string Title, string Description, IReadOnlyList<DemoPageLink> Pages);
