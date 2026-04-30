using System.Collections.Generic;
using Soenneker.Lepton.Suite;

namespace Soenneker.Bradix;

///<inheritdoc cref="IBradixComponent"/>
public abstract class BradixComponent : LeptonIdentifiableContentElement, IBradixComponent
{
    protected Dictionary<string, object> BuildAttributes(string key1, object? value1, string key2, object? value2, string key3, object? value3)
    {
        return base.BuildAttributes(
        [
            new KeyValuePair<string, object?>(key1, value1),
            new KeyValuePair<string, object?>(key2, value2),
            new KeyValuePair<string, object?>(key3, value3)
        ]);
    }

    protected static string OpenDataState(bool open) => BradixDataStates.Open(open);
}
