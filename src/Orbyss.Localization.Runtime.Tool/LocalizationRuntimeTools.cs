using System.ComponentModel;
using ModelContextProtocol.Server;

namespace Orbyss.Localization.Runtime.Tool;

/// <summary>Exposes read-only immutable localization runtime operations.</summary>
[McpServerToolType]
public sealed class LocalizationRuntimeTools
{
    /// <summary>Prevents direct construction; the SDK binds static tool methods.</summary>
    private LocalizationRuntimeTools() { }

    /// <summary>Resolves one runtime message through immutable fallback policy.</summary>
    [McpServerTool(Name = "localization.runtime.resolve", Title = "Resolve localized message", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false, UseStructuredContent = true)]
    [Description("Resolve one immutable localized message through the catalog fallback chain.")]
    public static ValueTask<LocalizationResolution?> ResolveAsync(
        LocalizationScope scope,
        string key,
        string languageTag,
        ILocalizationRuntime runtime,
        CancellationToken cancellationToken) =>
        runtime.ResolveAsync(scope, key, languageTag, cancellationToken);

    /// <summary>Gets one runtime bundle for a scope and language.</summary>
    [McpServerTool(Name = "localization.runtime.bundle", Title = "Get localization bundle", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false, UseStructuredContent = true)]
    [Description("Get one immutable localized message bundle for a structured scope and language.")]
    public static ValueTask<LocalizationBundle?> BundleAsync(
        LocalizationScope scope,
        string languageTag,
        ILocalizationRuntime runtime,
        CancellationToken cancellationToken) =>
        runtime.GetBundleAsync(scope, languageTag, cancellationToken);
}
