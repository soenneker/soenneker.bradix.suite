using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Soenneker.Bradix;

/// <summary>
/// Describes the public API of <see cref="BradixAccordionTrigger"/>.
/// </summary>
public interface IBradixAccordionTrigger : IDisposable, IAsyncDisposable {
    /// <summary>
    /// Gets or sets the callback invoked when a key is pressed on the trigger.
    /// </summary>
    EventCallback<KeyboardEventArgs> OnKeyDown { get; set; }
}
