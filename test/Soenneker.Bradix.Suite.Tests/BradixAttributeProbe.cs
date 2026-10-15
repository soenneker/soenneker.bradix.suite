using System.Collections.Generic;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixAttributeProbe : BradixIdentifiableElement
{
    public Dictionary<string, object> BuildEmpty() => BuildAttributes();

    public Dictionary<string, object> BuildPopulated() => BuildAttributes(
        ("role", "button"), ("data-state", "open"), ("data-a", "a"), ("data-b", "b"),
        ("data-c", "c"), ("data-d", "d"), ("data-e", "e"), ("data-f", "f"));
}
