using CShells.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Orbyss.Localization;

/// <summary>Composes a complete process-local localization backend for development and tests.</summary>
[ShellFeature(
    name: "Orbyss.Localization.Storage.InMemory",
    DisplayName = "Orbyss Localization In-Memory Storage",
    Description = "Provides bounded process-local catalog and release storage.")]
public sealed class OrbyssLocalizationInMemoryStorageFeature : IShellFeature
{
    /// <inheritdoc />
    public void ConfigureServices(IServiceCollection services)
    {
        services.TryAddSingleton(new InMemoryLocalizationStorageOptions());
        services.TryAddSingleton<InMemoryLocalizationCatalogStore>();
        services.TryAddSingleton<InMemoryLocalizationReleaseStore>();
        services.TryAddSingleton<ILocalizationCatalogStore>(provider => provider.GetRequiredService<InMemoryLocalizationCatalogStore>());
        services.TryAddSingleton<ILocalizationReleaseStore>(provider => provider.GetRequiredService<InMemoryLocalizationReleaseStore>());
        services.TryAddSingleton<ILocalizationReleaseRetirementStore>(provider => provider.GetRequiredService<InMemoryLocalizationReleaseStore>());
    }
}
