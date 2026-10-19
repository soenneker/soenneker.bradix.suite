using Soenneker.Lepton.Suite;

namespace Soenneker.Bradix;

/// <summary>Bradix component base with generated parameter bindings.</summary>
public abstract partial class BradixComponent : LeptonComponent
{
    protected override bool UseSharedParameterMetadata => true;
}
