using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;

namespace Soenneker.Bradix;

public sealed class BradixInput<TValue> : InputBase<TValue>, IBradixInput<TValue>
{
    [Parameter] public RenderFragment<BradixInputContext<TValue>>? ChildContent { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (ChildContent is null) return;

        string[] messages = EditContext?.GetValidationMessages(FieldIdentifier).ToArray() ?? [];
        var attributes = AdditionalAttributes is null
            ? new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
            : new Dictionary<string, object>(AdditionalAttributes, StringComparer.OrdinalIgnoreCase);
        if (!string.IsNullOrEmpty(CssClass)) attributes["class"] = CssClass;
        if (!attributes.ContainsKey("name") && !string.IsNullOrEmpty(NameAttributeValue))
            attributes["name"] = NameAttributeValue;
        if (messages.Length > 0) attributes["aria-invalid"] = "true";

        builder.AddContent(0, ChildContent(new BradixInputContext<TValue>(CurrentValue,
            EventCallback.Factory.Create<TValue>(this, SetValue), attributes, messages)));
    }

    private void SetValue(TValue value) => CurrentValue = value;

    protected override bool TryParseValueFromString(string? value, out TValue result, [NotNullWhen(false)] out string? validationErrorMessage)
        => throw new NotSupportedException("BradixInput accepts typed values through its template's ValueChanged callback.");
}
