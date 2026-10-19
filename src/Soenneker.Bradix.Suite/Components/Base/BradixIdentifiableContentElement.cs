using Microsoft.AspNetCore.Components;
using Soenneker.Lepton.Suite.Abstract;

namespace Soenneker.Bradix;

public abstract partial class BradixIdentifiableContentElement : BradixIdentifiableElement, ILeptonIdentifiableContentElement
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
}
