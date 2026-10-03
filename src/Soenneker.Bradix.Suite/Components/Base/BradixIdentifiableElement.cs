using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Soenneker.Lepton.Suite.Abstract;

namespace Soenneker.Bradix;

public abstract class BradixIdentifiableElement : BradixElement, ILeptonIdentifiableElement
{
    [Parameter]
    public string? Id { get; set; }

    protected override string? AttributeId => Id;

    protected IReadOnlyDictionary<string, object> EffectiveAttributes => BuildAttributes();
}

