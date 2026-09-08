# Orbyss.Localization.Runtime

Default immutable runtime resolution over an explicitly selected localization release store. The
feature selects the latest non-retired release for the configured catalog and registers
`ILocalizationRuntime`, so runtime consumers do not hand-wire the resolver.

Management, formats, storage implementations, web endpoints, and MCP integration remain separate.
Contract-only modular components should reference `Orbyss.Localization.Abstractions`.
