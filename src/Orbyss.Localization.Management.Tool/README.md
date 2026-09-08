# Orbyss.Localization.Management.Tool

Explicit closed-world MCP tools over the same catalog, import/export, lifecycle, and release-diff
services used by the Localization management HTTP feature. Import/export transfer is base64 encoded
and strictly bounded. Exact idempotency inputs are explicit and actors come only from the validated
transport principal. Read-only runtime resolution lives in `Orbyss.Localization.Runtime.Tool`.
