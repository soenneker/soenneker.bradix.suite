using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Soenneker.Blazor.Utils.ModuleImport.Registrars;
using Soenneker.Blazor.Utils.ResourceLoader.Registrars;

namespace Soenneker.Bradix;

/// <summary>
/// Registration for the interop and utility services.
/// </summary>
public static class BradixSuiteRegistrar
{
    public static IServiceCollection AddBradixSuiteAsScoped(this IServiceCollection services)
    {
        services.AddModuleImportUtilAsScoped()
                .AddResourceLoaderAsScoped();
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
