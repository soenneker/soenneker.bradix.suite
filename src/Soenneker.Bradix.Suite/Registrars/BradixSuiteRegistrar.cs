using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Soenneker.Blazor.Interops.Floating.Registrars;
using Soenneker.Bradix.Configuration;

namespace Soenneker.Bradix;

/// <summary>
/// Registration for the interop and utility services.
/// </summary>
public static class BradixSuiteRegistrar
{
    /// <summary>
    /// Adds bradix suite as scoped.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The result of the operation.</returns>
    public static IServiceCollection AddBradixSuiteAsScoped(this IServiceCollection services)
    {
        return services.AddBradixSuiteAsScoped(null);
    }

    /// <summary>
    /// Adds bradix suite as scoped.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">The configure.</param>
    /// <returns>The result of the operation.</returns>
    public static IServiceCollection AddBradixSuiteAsScoped(this IServiceCollection services, Action<BradixSuiteOptions>? configure)
    {
        services.AddFloatingUiInteropAsScoped();
        services.AddOptions<BradixSuiteOptions>();

        if (configure is not null)
            services.Configure(configure);

        services.TryAddScoped<IBradixSuiteInterop, BradixSuiteInterop>();
        services.TryAddScoped<ICollapsibleInterop, CollapsibleInterop>();
        services.TryAddScoped<IControlsInterop, ControlsInterop>();
        services.TryAddScoped<IDelegatedInteractionInterop, DelegatedInteractionInterop>();
        services.TryAddScoped<IDismissableLayerInterop, DismissableLayerInterop>();
        services.TryAddScoped<IDomInterop, DomInterop>();
        services.TryAddScoped<IFocusScopeInterop, FocusScopeInterop>();
        services.TryAddScoped<IFormInterop, FormInterop>();
        services.TryAddScoped<IHoverCardAvatarInterop, HoverCardAvatarInterop>();
        services.TryAddScoped<IKeyboardModeInterop, KeyboardModeInterop>();
        services.TryAddScoped<ILabelInterop, LabelInterop>();
        services.TryAddScoped<IMenuInterop, MenuInterop>();
        services.TryAddScoped<IMenubarInterop, MenubarInterop>();
        services.TryAddScoped<INavigationMenuInterop, NavigationMenuInterop>();
        services.TryAddScoped<IPopperInterop, PopperInterop>();
        services.TryAddScoped<IPortalInterop, PortalInterop>();
        services.TryAddScoped<IPresenceOverlayInterop, PresenceOverlayInterop>();
        services.TryAddScoped<IRadioGroupInterop, RadioGroupInterop>();
        services.TryAddScoped<IRovingFocusInterop, RovingFocusInterop>();
        services.TryAddScoped<IScrollAreaInterop, ScrollAreaInterop>();
        services.TryAddScoped<ISelectInterop, SelectInterop>();
        services.TryAddScoped<IToastInterop, ToastInterop>();
        services.TryAddScoped<ITooltipInterop, TooltipInterop>();

        return services;
    }
}
