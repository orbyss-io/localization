# Orbyss Localization

Provider-neutral localization authoring, validation, import/export, immutable release, runtime,
storage, web, and MCP building blocks maintained by Orbyss.

This repository owns exactly `Orbyss.Localization` and the `Orbyss.Localization.*` .NET package family. It depends on released
`Orbyss.Foundation.*` infrastructure packages but has no dependency on Orbyss Forms or Program Kit.
The Forms-specific adapter remains in the Forms repository as `Orbyss.Forms.Localization`.

## Package family

- A dependency-free abstractions package for modular consumers.
- A primary implementation package with default validation and immutable runtime behavior.
- Feature-based application lifecycle composition so consumers do not hand-wire the service graph.
- CSV, JSON, PO, XLIFF 2.1, and XLSX import/export adapters.
- Replaceable filesystem and bounded in-memory storage.
- Optional CShells web management/runtime features and shared Foundation MCP tools.

## Build

```powershell
dotnet restore Orbyss.Localization.slnx --locked-mode --configfile NuGet.config
dotnet build Orbyss.Localization.slnx -c Release --no-restore
python tests/validate_localization_contracts.py
python tests/validate_localization_management.py
python tests/validate_inmemory_storage.py
```

Stable tags must equal `v` plus the exact value in `VERSION`. The protected Release workflow packs,
attests, publishes, and verifies the complete 11-package family through NuGet trusted publishing.
