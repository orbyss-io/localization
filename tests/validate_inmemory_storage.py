from __future__ import annotations

import subprocess
from pathlib import Path


def main() -> int:
    root = Path(__file__).resolve().parents[1]
    storage = root / "src/Orbyss.Localization.Storage.InMemory"
    source = "\n".join(path.read_text(encoding="utf-8") for path in storage.glob("*.cs"))
    for contract in ("ILocalizationCatalogStore", "ILocalizationReleaseStore", "ILocalizationReleaseRetirementStore"):
        if contract not in source:
            raise AssertionError(f"The in-memory Localization adapter does not implement {contract}")
    probe = root / "tests/Orbyss.Localization.Storage.InMemory.Probe/Orbyss.Localization.Storage.InMemory.Probe.csproj"
    result = subprocess.run(
        ["dotnet", "run", "--project", str(probe), "--configuration", "Release", "--no-build"],
        cwd=root,
        capture_output=True,
        text=True,
        timeout=180,
        check=False,
    )
    if result.returncode:
        raise AssertionError(f"Localization in-memory probe failed.\nstdout:\n{result.stdout}\nstderr:\n{result.stderr}")
    print("Orbyss Localization in-memory persistence validation passed.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
