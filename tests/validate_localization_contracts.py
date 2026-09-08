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
    projects = sorted((root / "src/dotnet").glob("Orbyss.Localization.*/*.csproj"))
    if len(projects) != 11:
        raise AssertionError(f"Expected 11 Localization projects, found {len(projects)}")
    for project in projects:
        relative = project.relative_to(root).as_posix()
        if relative not in solution:
            raise AssertionError(f"Localization project is outside the solution: {relative}")
    if any((root / "src/dotnet").glob("Orbyss.Forms.*")):
        raise AssertionError("Localization must not own Forms source.")

    abstractions = root / "src/dotnet/Orbyss.Localization.Abstractions/Orbyss.Localization.Abstractions.csproj"
    if references(abstractions, "PackageReference") or references(abstractions, "ProjectReference"):
        raise AssertionError("Localization semantic contracts leaked dependencies.")

    graphs = {
        "Orbyss.Localization.Core": {"../Orbyss.Localization.Abstractions/Orbyss.Localization.Abstractions.csproj"},
        "Orbyss.Localization.Application": {
            "../Orbyss.Localization.Abstractions/Orbyss.Localization.Abstractions.csproj",
            "../Orbyss.Localization.Core/Orbyss.Localization.Core.csproj",
            "../Orbyss.Localization.Storage.Abstractions/Orbyss.Localization.Storage.Abstractions.csproj",
        },
        "Orbyss.Localization.Formats": {"../Orbyss.Localization.Abstractions/Orbyss.Localization.Abstractions.csproj"},
    }
    for name, expected in graphs.items():
        project = root / f"src/dotnet/{name}/{name}.csproj"
        if references(project, "ProjectReference") != expected:
            raise AssertionError(f"Unexpected project graph for {name}")

    run_probe(root, "Orbyss.Localization.Contracts.Probe")
    run_probe(root, "Orbyss.Localization.Web.Probe")
    print("Orbyss Localization contract, format, lifecycle, and web validation passed.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
