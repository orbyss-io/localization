# Contributor instructions

- Localization owns exactly the `Orbyss.Localization.*` .NET package family and its validation and publication suites.
- Localization may depend on released Orbyss Foundation packages; it must not depend on Forms or Program Kit source.
- Keep localization semantics provider-neutral and keep storage, web, and MCP adapters replaceable and thin.
- Run locked .NET restore/build, contract probes, and exact 11-package validation before tagging.
- A stable tag is only available after the complete protected Release workflow succeeds.
