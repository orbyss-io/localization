# Changelog

## 0.1.1

- Replace ambiguous `Orbyss.Localization` and `Orbyss.Localization.Application` packages with explicit Runtime and Management implementations.
- Split privileged management MCP tools from read-only runtime MCP tools and contributors.
- Keep storage implementations independent from management and format packages.
- Ensure every implementation surface provides exact CShells feature composition.
- Flatten .NET probes directly beneath `tests` and guard against `src/dotnet`, `tests/dotnet`, and `eng` regressions.
- Expand exact-family validation and trusted publication gates to all 13 packages.

## 0.1.0

- Extract the independent `Orbyss.Localization` package family from Orbyss Forms.
- Keep dependency-free `Orbyss.Localization.Abstractions` contracts and rename the former core implementation package to `Orbyss.Localization`.
- Add application, formats, filesystem, and in-memory composition features.
- Preserve provider-neutral contracts, lifecycle, formats, storage, web, tool, and MCP capabilities.
- Add exact-family validation and trusted NuGet publication gates for all 11 packages.
