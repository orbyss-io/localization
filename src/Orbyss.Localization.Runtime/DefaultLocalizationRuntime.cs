namespace Orbyss.Localization;

/// <summary>Resolves messages from the latest non-retired release in the selected store.</summary>
public sealed class DefaultLocalizationRuntime(
    ILocalizationReleaseStore releases,
    LocalizationRuntimeOptions options) : ILocalizationRuntime
{
    /// <inheritdoc />
    public async ValueTask<LocalizationResolution?> ResolveAsync(
        LocalizationScope scope,
        string key,
        string languageTag,
        CancellationToken cancellationToken = default)
    {
        var runtime = await LoadAsync(cancellationToken).ConfigureAwait(false);
        return runtime is null
            ? null
            : await runtime.ResolveAsync(scope, key, languageTag, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask<LocalizationBundle?> GetBundleAsync(
        LocalizationScope scope,
        string languageTag,
        CancellationToken cancellationToken = default)
    {
        var runtime = await LoadAsync(cancellationToken).ConfigureAwait(false);
        return runtime is null
            ? null
            : await runtime.GetBundleAsync(scope, languageTag, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>Loads the latest live release without exposing storage details to consumers.</summary>
    private async ValueTask<InMemoryLocalizationRuntime?> LoadAsync(CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(releases);
        ArgumentNullException.ThrowIfNull(options);
        ArgumentException.ThrowIfNullOrWhiteSpace(options.CatalogId);
        LocalizationRelease? selected = null;
        await foreach (var release in releases.FindByCatalogAsync(
            new LocalizationCatalogId(options.CatalogId),
            cancellationToken).ConfigureAwait(false))
        {
            if (!release.Retired)
            {
                selected = release;
            }
        }

        return selected is null ? null : new InMemoryLocalizationRuntime(selected);
    }
}
