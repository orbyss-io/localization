using CShells;
using CShells.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using ModelContextProtocol.Server;
using Orbyss.Localization.Management.Tool;
using Orbyss.Foundation.Mcp.AspNetCore;

namespace Orbyss.Localization.Management.Mcp.AspNetCore;

/// <summary>Contributes governed localization tools to the shared MCP transport.</summary>
[ShellFeature(
    name: "Orbyss.Localization.Management.Mcp.AspNetCore",
    DisplayName = "Orbyss Localization Management MCP",
    Description = "Contributes governed localization catalog and release-management tools.",
    DependsOn = [typeof(FoundationMcpFeature), typeof(OrbyssLocalizationManagementFeature)])]
public sealed class OrbyssLocalizationManagementMcpFeature(ShellSettings settings) : IShellFeature
{
    /// <inheritdoc />
    public void ConfigureServices(IServiceCollection services)
    {
        services.Configure<LocalizationManagementMcpOptions>(settings.GetConfigurationRoot().GetSection(LocalizationManagementMcpOptions.SectionName));
        services.TryAddSingleton<ILocalizationToolActorProvider>(provider =>
        {
            var options = RequiredOptions(provider);
            return new ClaimsLocalizationToolActorProvider(new LocalizationToolIdentityOptions(options.SubjectClaimType, options.ActorKindClaimType, options.DisplayNameClaimType));
        });
        services.TryAddSingleton(provider =>
        {
            var options = RequiredOptions(provider);
            return new LocalizationToolTransferOptions(options.MaximumImportBytes, options.MaximumExportBytes);
        });
        services.AddMcpServer().WithTools<LocalizationManagementTools>();
    }

    /// <summary>Resolves and validates contributor options.</summary>
    private static LocalizationManagementMcpOptions RequiredOptions(IServiceProvider provider)
    {
        var options = provider.GetRequiredService<IOptions<LocalizationManagementMcpOptions>>().Value;
        if (string.IsNullOrWhiteSpace(options.SubjectClaimType) || string.IsNullOrWhiteSpace(options.ActorKindClaimType) || string.IsNullOrWhiteSpace(options.DisplayNameClaimType)) throw new InvalidOperationException("Localization MCP claim mappings are required.");
        if (options.MaximumImportBytes is < 1 or > 67_108_864 || options.MaximumExportBytes is < 1 or > 67_108_864) throw new InvalidOperationException("Localization MCP transfer limits must be from one byte through 64 MiB.");
        return options;
    }
}
