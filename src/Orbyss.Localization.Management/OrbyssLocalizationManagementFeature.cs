using Orbyss.Localization.Formats;
using CShells.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Orbyss.Localization;

/// <summary>Registers the default localization validation, import coordination, lifecycle, and query services.</summary>
[ShellFeature(
    name: "Orbyss.Localization.Management",
    DisplayName = "Orbyss Localization Management",
    Description = "Provides catalog authoring, validation, import, lifecycle, publication, and queries over selected storage.",
    DependsOn = [typeof(OrbyssLocalizationFormatsFeature)])]
public sealed class OrbyssLocalizationManagementFeature : IShellFeature
{
    /// <inheritdoc />
    public void ConfigureServices(IServiceCollection services)
    {
        services.TryAddSingleton(new LocalizationValidationOptions());
        services.TryAddSingleton<ILocalizationCatalogValidator>(provider =>
            new LocalizationCatalogValidator(provider.GetRequiredService<LocalizationValidationOptions>()));
        services.TryAddSingleton<ILocalizationReleaseDiffer, DefaultLocalizationReleaseDiffer>();
        services.TryAddSingleton(new LocalizationImportCoordinatorOptions());
        services.TryAddSingleton<LocalizationImportCoordinator>();
        services.TryAddSingleton<DefaultLocalizationCatalogService>();
        services.TryAddSingleton<ILocalizationCatalogManagement>(provider => provider.GetRequiredService<DefaultLocalizationCatalogService>());
        services.TryAddSingleton<ILocalizationCatalogQueries>(provider => provider.GetRequiredService<DefaultLocalizationCatalogService>());
        services.TryAddSingleton<ILocalizationReleaseLifecycle>(provider => provider.GetRequiredService<DefaultLocalizationCatalogService>());
    }
}
