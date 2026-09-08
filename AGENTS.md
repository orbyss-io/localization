# Contributor instructions

- Localization owns exactly `Orbyss.Localization` and the `Orbyss.Localization.*` .NET package family and its validation and publication suites.
- Localization may depend on released Orbyss Foundation packages; it must not depend on Forms or Program Kit source.
- Keep localization semantics provider-neutral and keep storage, web, and MCP adapters replaceable and thin.
- Run locked .NET restore/build, feature-composition probes, and exact 13-package validation before tagging.
- A stable tag is only available after the complete protected Release workflow succeeds.
