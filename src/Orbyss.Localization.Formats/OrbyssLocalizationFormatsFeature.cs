using CShells.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Orbyss.Localization.Formats;

/// <summary>Registers every bounded built-in localization import and export adapter.</summary>
[ShellFeature(
    name: "Orbyss.Localization.Formats",
    DisplayName = "Orbyss Localization Formats",
    Description = "Provides bounded CSV, JSON, PO, XLIFF 2.1, and XLSX import and export adapters.")]
public sealed class OrbyssLocalizationFormatsFeature : IShellFeature
{
    /// <inheritdoc />
    public void ConfigureServices(IServiceCollection services)
    {
        services.TryAddSingleton(new LocalizationImportFormatOptions());
        services.TryAddSingleton(new LocalizationExportFormatOptions());
        AddImport<CsvLocalizationImportAdapter>(services);
        AddImport<JsonLocalizationImportAdapter>(services);
        AddImport<PoLocalizationImportAdapter>(services);
        AddImport<Xliff21LocalizationImportAdapter>(services);
        AddImport<XlsxLocalizationImportAdapter>(services);
        AddExport<CsvLocalizationExportAdapter>(services);
        AddExport<JsonLocalizationExportAdapter>(services);
        AddExport<PoLocalizationExportAdapter>(services);
        AddExport<Xliff21LocalizationExportAdapter>(services);
        AddExport<XlsxLocalizationExportAdapter>(services);
    }

    /// <summary>Adds one built-in import adapter without replacing a consumer override.</summary>
    private static void AddImport<T>(IServiceCollection services) where T : class, ILocalizationImportFormatAdapter =>
        services.TryAddEnumerable(ServiceDescriptor.Singleton<ILocalizationImportFormatAdapter, T>());

    /// <summary>Adds one built-in export adapter without replacing a consumer override.</summary>
    private static void AddExport<T>(IServiceCollection services) where T : class, ILocalizationExportFormatAdapter =>
        services.TryAddEnumerable(ServiceDescriptor.Singleton<ILocalizationExportFormatAdapter, T>());
}
