using CShells;
using CShells.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Orbyss.Localization.Formats;

namespace Orbyss.Localization;

/// <summary>Composes a complete durable filesystem localization backend.</summary>
[ShellFeature(
    name: "Orbyss.Localization.Storage.FileSystem",
    DisplayName = "Orbyss Localization File-System Storage",
    Description = "Provides atomic content-verified catalog and immutable-release persistence.",
    DependsOn = [typeof(OrbyssLocalizationApplicationFeature), typeof(OrbyssLocalizationFormatsFeature)])]
public sealed class OrbyssLocalizationFileSystemStorageFeature(ShellSettings settings) : IShellFeature
{
    /// <summary>Configuration section for the owned storage root.</summary>
    public const string SectionName = "Orbyss:Localization:Storage:FileSystem";

    /// <inheritdoc />
    public void ConfigureServices(IServiceCollection services)
    {
        var configuredRoot = settings.GetConfigurationRoot().GetSection(SectionName)["RootPath"];
        var root = Path.GetFullPath(string.IsNullOrWhiteSpace(configuredRoot)
            ? Path.Combine(AppContext.BaseDirectory, "App_Data", "localization")
            : configuredRoot);
        services.TryAddSingleton(new FileSystemLocalizationCatalogStoreOptions(Path.Combine(root, "catalogs")));
        services.TryAddSingleton(new FileSystemLocalizationReleaseStoreOptions(Path.Combine(root, "releases")));
        services.TryAddSingleton<FileSystemLocalizationCatalogStore>();
        services.TryAddSingleton<FileSystemLocalizationReleaseStore>();
        services.TryAddSingleton<ILocalizationCatalogStore>(provider => provider.GetRequiredService<FileSystemLocalizationCatalogStore>());
        services.TryAddSingleton<ILocalizationReleaseStore>(provider => provider.GetRequiredService<FileSystemLocalizationReleaseStore>());
        services.TryAddSingleton<ILocalizationReleaseRetirementStore>(provider => provider.GetRequiredService<FileSystemLocalizationReleaseStore>());
    }
}
