using Orbyss.Localization;

var localizationActor = new LocalizationAuditActor("translator-1", "user");
var catalog = new LocalizationCatalogDefinition(new LocalizationCatalogId("application"), new LocalizationRevision(1), "Application", "en", LocalizationLifecycleState.Draft, [new LocaleDefinition("en", TextDirection.LeftToRight, RequiredForPublication: true)], [], new Dictionary<string, string> { ["team"] = "core" });
var catalogs = new InMemoryLocalizationCatalogStore();
var catalogCreated = await catalogs.WriteAsync(new LocalizationCatalogPersistenceCommand("create", "fp-catalog", catalog, LocalizationMutation("catalog-create", null, localizationActor, 9), true));
var catalogReplay = await catalogs.ReplayAsync(catalog.Id, "catalog-create", "fp-catalog");
Require(catalogReplay?.WasReplay == true && catalogReplay.Snapshot.Version == catalogCreated.Snapshot.Version, "localization catalog replay failed");
await RequireThrowsAsync<InvalidOperationException>(() => catalogs.WriteAsync(new LocalizationCatalogPersistenceCommand("replace", "fp-catalog-stale", catalog with { Revision = new LocalizationRevision(2) }, LocalizationMutation("catalog-stale", new LocalizationConcurrencyToken("stale"), localizationActor, 10))).AsTask(), "localization catalog accepted a stale write");
var boundedCatalogs = new InMemoryLocalizationCatalogStore(new InMemoryLocalizationStorageOptions { MaximumDocumentBytes = 1024 });
var oversizedCatalog = catalog with { Metadata = new Dictionary<string, string> { ["oversized"] = new string('x', 2048) } };
await RequireThrowsAsync<InvalidOperationException>(() => boundedCatalogs.WriteAsync(new LocalizationCatalogPersistenceCommand("create", "fp-catalog-oversized", oversizedCatalog, LocalizationMutation("catalog-oversized", null, localizationActor, 10), true)).AsTask(), "oversized in-memory localization catalog was accepted");

var localizationRelease = new LocalizationRelease(new LocalizationReleaseId("application-r1"), catalog.Id, catalog.Revision, "en", catalog.Locales, [], "localization-sha", DateTimeOffset.UnixEpoch.AddMinutes(11), localizationActor);
var localizationReleases = new InMemoryLocalizationReleaseStore();
await localizationReleases.WriteAsync(localizationRelease);
var localizationRetired = await localizationReleases.RetireAsync(localizationRelease.Id, "fp-localization-retire", LocalizationMutation("localization-retire", new LocalizationConcurrencyToken(localizationRelease.Sha256), localizationActor, 12));
var localizationRetireReplay = await localizationReleases.RetireAsync(localizationRelease.Id, "fp-localization-retire", LocalizationMutation("localization-retire", new LocalizationConcurrencyToken(localizationRelease.Sha256), localizationActor, 12));
Require(localizationRetired.Value.Retired && localizationRetireReplay.WasReplay && (await ReadAllAsync(localizationReleases.FindByCatalogAsync(catalog.Id))).Single().Retired, "localization release retirement failed");


Console.WriteLine("Orbyss Localization in-memory storage probe passed.");

static LocalizationMutationContext LocalizationMutation(string key, LocalizationConcurrencyToken? version, LocalizationAuditActor actor, int minute) => new(key, version, actor, DateTimeOffset.UnixEpoch.AddMinutes(minute), "probe");
static async Task<List<T>> ReadAllAsync<T>(IAsyncEnumerable<T> values) { var result = new List<T>(); await foreach (var value in values) result.Add(value); return result; }
static void Require(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); }
static async Task RequireThrowsAsync<T>(Func<Task> action, string message) where T : Exception { try { await action(); } catch (T) { return; } throw new InvalidOperationException(message); }
