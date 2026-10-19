using Microsoft.AspNetCore.Components;
using Soenneker.Lepton.Suite.Abstract;

namespace Soenneker.Bradix;

public abstract partial class BradixContentElement : BradixElement, ILeptonContentElement
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
}

