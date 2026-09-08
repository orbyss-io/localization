# Orbyss.Localization

The primary provider-neutral Localization implementation package. It builds on
`Orbyss.Localization.Abstractions` and provides default validation, import coordination, release
comparison, and immutable in-memory runtime behavior.

Storage, application orchestration, concrete file formats, web endpoints, and MCP integration remain
separate optional packages so consumers pay only for the adapters they select. Modular components
that need contracts only should reference `Orbyss.Localization.Abstractions` directly.
