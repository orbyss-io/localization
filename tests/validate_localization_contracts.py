from __future__ import annotations

import subprocess
from pathlib import Path
from xml.etree import ElementTree


def references(project: Path, kind: str) -> set[str]:
    tree = ElementTree.parse(project)
    return {
        item.attrib["Include"].replace("\\", "/")
        for item in tree.iter()
        if item.tag.endswith(kind)
    }


def run_probe(root: Path, name: str) -> None:
    project = root / f"tests/dotnet/{name}/{name}.csproj"
    result = subprocess.run(
        ["dotnet", "run", "--project", str(project), "--configuration", "Release", "--no-build"],
        cwd=root,
        capture_output=True,
        text=True,
        timeout=180,
        check=False,
    )
    if result.returncode:
        raise AssertionError(f"{name} failed.\nstdout:\n{result.stdout}\nstderr:\n{result.stderr}")


def main() -> int:
    root = Path(__file__).resolve().parents[1]
    solution = (root / "Orbyss.Localization.slnx").read_text(encoding="utf-8").replace("\\", "/")
    projects = sorted((root / "src").glob("Orbyss.Localization*/*.csproj"))
    if len(projects) != 11:
        raise AssertionError(f"Expected 11 Localization projects, found {len(projects)}")
    for project in projects:
        relative = project.relative_to(root).as_posix()
        if relative not in solution:
            raise AssertionError(f"Localization project is outside the solution: {relative}")
    if (root / "src/dotnet").exists() or (root / "eng").exists():
        raise AssertionError("Localization must use the flat src layout and must not contain an eng layer.")
    if any((root / "src").glob("Orbyss.Forms.*")):
        raise AssertionError("Localization must not own Forms source.")

    abstractions = root / "src/Orbyss.Localization.Abstractions/Orbyss.Localization.Abstractions.csproj"
    if references(abstractions, "PackageReference") or references(abstractions, "ProjectReference"):
        raise AssertionError("Localization abstractions leaked implementation dependencies.")

    expected_graphs = {
        "Orbyss.Localization.Application": {
            "../Orbyss.Localization/Orbyss.Localization.csproj",
            "../Orbyss.Localization.Storage.Abstractions/Orbyss.Localization.Storage.Abstractions.csproj",
        },
        "Orbyss.Localization": {"../Orbyss.Localization.Abstractions/Orbyss.Localization.Abstractions.csproj"},
        "Orbyss.Localization.Formats": {"../Orbyss.Localization.Abstractions/Orbyss.Localization.Abstractions.csproj"},
        "Orbyss.Localization.Storage.Abstractions": {"../Orbyss.Localization.Abstractions/Orbyss.Localization.Abstractions.csproj"},
    }
    for name, expected in expected_graphs.items():
        project = root / f"src/{name}/{name}.csproj"
        if references(project, "ProjectReference") != expected:
            raise AssertionError(f"Unexpected project graph for {name}")

    features = {
        "src/Orbyss.Localization/OrbyssLocalizationFeature.cs": "ILocalizationCatalogValidator",
        "src/Orbyss.Localization.Application/OrbyssLocalizationApplicationFeature.cs": "ILocalizationCatalogManagement",
        "src/Orbyss.Localization.Formats/OrbyssLocalizationFormatsFeature.cs": "ILocalizationImportFormatAdapter",
        "src/Orbyss.Localization.Storage.FileSystem/OrbyssLocalizationFileSystemStorageFeature.cs": "ILocalizationCatalogStore",
        "src/Orbyss.Localization.Storage.InMemory/OrbyssLocalizationInMemoryStorageFeature.cs": "ILocalizationCatalogStore",
    }
    for relative, registration in features.items():
        source = (root / relative).read_text(encoding="utf-8")
        if "ShellFeature(" not in source or "ConfigureServices" not in source or registration not in source:
            raise AssertionError(f"Localization implementation package lacks usable feature composition: {relative}")

    run_probe(root, "Orbyss.Localization.Contracts.Probe")
    run_probe(root, "Orbyss.Localization.Web.Probe")
    print("Orbyss Localization contract, feature, format, lifecycle, and web validation passed.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
