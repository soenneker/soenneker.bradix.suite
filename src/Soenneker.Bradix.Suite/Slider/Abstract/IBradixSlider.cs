using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Defines the public API for <see cref="BradixSlider"/>.
/// </summary>
public interface IBradixSlider : IAsyncDisposable {
    /// <summary>Gets or sets the element identifier.</summary>
    string? Id { get; set; }

    /// <summary>Gets or sets the CSS class.</summary>
    string? Class { get; set; }

    /// <summary>Gets or sets the inline style.</summary>
    string? Style { get; set; }

    /// <summary>Gets or sets the child content.</summary>
    RenderFragment? ChildContent { get; set; }

    /// <summary>Gets or sets additional attributes merged onto the root element.</summary>
    IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>Gets or sets the field name for native inputs.</summary>
    string? Name { get; set; }

    /// <summary>Gets or sets whether interaction is disabled.</summary>
    bool Disabled { get; set; }

    /// <summary>Gets or sets the slider orientation.</summary>
    string Orientation { get; set; }

    /// <summary>Gets or sets the text direction override.</summary>
    string? Dir { get; set; }

    /// <summary>Gets or sets the minimum value.</summary>
    double Min { get; set; }

    /// <summary>Gets or sets the maximum value.</summary>
    double Max { get; set; }

    /// <summary>Gets or sets the step between valid values.</summary>
    double Step { get; set; }

    /// <summary>Gets or sets the minimum step distance allowed between thumbs.</summary>
    double MinStepsBetweenThumbs { get; set; }

    /// <summary>Gets or sets the controlled thumb values.</summary>
    IReadOnlyList<double>? Values { get; set; }

    /// <summary>Gets or sets the initial thumb values when uncontrolled.</summary>
    IEnumerable<double>? DefaultValues { get; set; }

    /// <summary>Gets or sets the callback invoked when values change (two-way bind).</summary>
    EventCallback<IReadOnlyList<double>> ValuesChanged { get; set; }

    /// <summary>Gets or sets the callback invoked when values change.</summary>
    EventCallback<IReadOnlyList<double>> OnValuesChange { get; set; }

    /// <summary>Gets or sets the callback invoked when a pointer gesture completes after a value change.</summary>
    EventCallback<IReadOnlyList<double>> OnValueCommit { get; set; }

    /// <summary>Gets or sets whether the value scale is inverted along the orientation axis.</summary>
    bool Inverted { get; set; }

    /// <summary>Gets or sets the <c>form</c> attribute for detached native inputs.</summary>
    string? Form { get; set; }


    /// <summary>Called from script when a pointer drag begins.</summary>
    Task HandlePointerStart(double xFraction, double yFraction, int thumbIndex);

    /// <summary>Called from script during a pointer drag.</summary>
    Task HandlePointerMove(double xFraction, double yFraction);

    /// <summary>Called from script when a pointer drag ends.</summary>
    Task HandlePointerEnd();

    /// <summary>Called from script when a pointer gesture is cancelled.</summary>
    Task HandlePointerCancel();
}
