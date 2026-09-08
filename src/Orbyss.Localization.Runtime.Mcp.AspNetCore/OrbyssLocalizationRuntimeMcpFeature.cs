using CShells.Features;
using Microsoft.Extensions.DependencyInjection;
using ModelContextProtocol.Server;
using Orbyss.Foundation.Mcp.AspNetCore;
using Orbyss.Localization.Runtime.Tool;

namespace Orbyss.Localization.Runtime.Mcp.AspNetCore;

/// <summary>Contributes read-only localization runtime tools to the shared MCP transport.</summary>
[ShellFeature(
    name: "Orbyss.Localization.Runtime.Mcp.AspNetCore",
    DisplayName = "Orbyss Localization Runtime MCP",
    Description = "Contributes read-only localization resolution and bundle tools.",
    DependsOn = [typeof(FoundationMcpFeature), typeof(OrbyssLocalizationRuntimeFeature)])]
public sealed class OrbyssLocalizationRuntimeMcpFeature : IShellFeature
{
    /// <inheritdoc />
    public void ConfigureServices(IServiceCollection services) =>
        services.AddMcpServer().WithTools<LocalizationRuntimeTools>();
}
