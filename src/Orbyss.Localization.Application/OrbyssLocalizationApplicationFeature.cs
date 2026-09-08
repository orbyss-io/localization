using CShells.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Orbyss.Localization;

/// <summary>Registers the default localization validation, import coordination, lifecycle, and query services.</summary>
[ShellFeature(
    name: "Orbyss.Localization.Application",
    DisplayName = "Orbyss Localization Application",
    Description = "Provides catalog lifecycle, publication, and queries over selected storage and format adapters.",
    DependsOn = [typeof(OrbyssLocalizationFeature)])]
public sealed class OrbyssLocalizationApplicationFeature : IShellFeature
{
    /// <inheritdoc />
    public void ConfigureServices(IServiceCollection services)
    {
        services.TryAddSingleton<DefaultLocalizationCatalogService>();
        services.TryAddSingleton<ILocalizationCatalogManagement>(provider => provider.GetRequiredService<DefaultLocalizationCatalogService>());
        services.TryAddSingleton<ILocalizationCatalogQueries>(provider => provider.GetRequiredService<DefaultLocalizationCatalogService>());
        services.TryAddSingleton<ILocalizationReleaseLifecycle>(provider => provider.GetRequiredService<DefaultLocalizationCatalogService>());
    }
}
