# Orbyss.Localization.Web.Management

An optional `CShells.IWebShellFeature` that maps localization catalog, validation, import,
export, lifecycle, release lookup, and release-diff endpoints. It does not add authentication,
authorization middleware, persistence, or a response-body policy.

The web feature depends on `Orbyss.Localization.Management`, which supplies the default management
services and built-in format adapters without replacing consumer overrides. Consumers still select
one storage feature or provide another implementation of the storage ports. All endpoints require authorization;
optional named policies separate read, write, import, and export permissions. Audit actor identity
is derived from configured authenticated claims rather than request content. Import uploads have
both a pre-binding request-body limit and a decoded-byte limit; the configurable decoded maximum is
8 MiB by default and cannot exceed 64 MiB.
