using Soenneker.Lepton.Suite;

namespace Soenneker.Bradix;

/// <summary>Bradix content base with generated parameter bindings.</summary>
public abstract partial class BradixContent : LeptonContent
{
    protected override bool UseSharedParameterMetadata =>
#if BRADIX_FRAMEWORK_PARAMETER_BINDING
        false;
#else
        true;
#endif
}
