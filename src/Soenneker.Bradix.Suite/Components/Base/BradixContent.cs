using Soenneker.Lepton.Suite;

namespace Soenneker.Bradix;

/// <summary>Bradix content base with generated parameter bindings.</summary>
public abstract partial class BradixContent : LeptonContent
{
    protected override bool UseSharedParameterMetadata => true;
}
