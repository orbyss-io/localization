using CShells;
using CShells.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Orbyss.Localization;

/// <summary>Registers immutable release-backed localization resolution.</summary>
[ShellFeature(
    name: "Orbyss.Localization.Runtime",
    DisplayName = "Orbyss Localization Runtime",
    Description = "Resolves messages and bundles from the latest non-retired release of one configured catalog.")]
public sealed class OrbyssLocalizationRuntimeFeature(ShellSettings settings) : IShellFeature
{
    /// <inheritdoc />
    public void ConfigureServices(IServiceCollection services)
    {
        var configuredCatalogId = settings.GetConfigurationRoot()
            .GetSection(LocalizationRuntimeOptions.SectionName)["CatalogId"];
        services.TryAddSingleton(new LocalizationRuntimeOptions(
            string.IsNullOrWhiteSpace(configuredCatalogId) ? "application" : configuredCatalogId));
        services.TryAddSingleton<ILocalizationRuntime, DefaultLocalizationRuntime>();
    }
}
