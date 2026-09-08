# Orbyss Localization

Provider-neutral localization authoring, validation, import/export, immutable release, runtime,
storage, web, and MCP building blocks maintained by Orbyss.

This repository owns exactly `Orbyss.Localization` and the `Orbyss.Localization.*` .NET package family. It depends on released
`Orbyss.Foundation.*` infrastructure packages but has no dependency on Orbyss Forms or Program Kit.
The Forms-specific adapter remains in the Forms repository as `Orbyss.Forms.Localization`.

## Package family

- `Orbyss.Localization.Abstractions` provides dependency-free contracts for modular consumers.
- `Orbyss.Localization.Management` composes authoring, validation, import, lifecycle, publication, queries, and release comparison.
- `Orbyss.Localization.Runtime` resolves messages and bundles from the latest non-retired release of one configured catalog.
- `Orbyss.Localization.Formats` provides CSV, JSON, PO, XLIFF 2.1, and XLSX import/export adapters.
- `Orbyss.Localization.Storage.*` keeps persistence ports, filesystem storage, and bounded in-memory storage replaceable.
- `Orbyss.Localization.Web.Management` and `.Web.Runtime` expose separately selectable HTTP capabilities.
- `Orbyss.Localization.Management.Tool`/`.Management.Mcp.AspNetCore` keep privileged tools separate from the read-only `Runtime.Tool`/`Runtime.Mcp.AspNetCore` pair.

Every concrete package exposes a CShells feature. Storage features register only their adapter,
Management depends on the built-in Formats feature, and the web/MCP features depend on their exact
Management or Runtime implementation feature.

## Build

```powershell
dotnet restore Orbyss.Localization.slnx --locked-mode --configfile NuGet.config
dotnet build Orbyss.Localization.slnx -c Release --no-restore
python tests/validate_localization_contracts.py
python tests/validate_localization_management.py
python tests/validate_inmemory_storage.py
```

Stable tags must equal `v` plus the exact value in `VERSION`. The protected Release workflow packs,
attests, publishes, and verifies the complete 13-package family through NuGet trusted publishing.
