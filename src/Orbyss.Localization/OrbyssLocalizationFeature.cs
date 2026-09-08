using CShells.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Orbyss.Localization;

/// <summary>Registers the default provider-neutral Localization implementations.</summary>
[ShellFeature(
    name: "Orbyss.Localization",
    DisplayName = "Orbyss Localization",
    Description = "Provides default catalog validation, import coordination, release comparison, and immutable runtime behavior.")]
public sealed class OrbyssLocalizationFeature : IShellFeature
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
    }
}
