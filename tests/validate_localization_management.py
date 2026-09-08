from __future__ import annotations

import re
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


def main() -> int:
    root = Path(__file__).resolve().parents[1]
    tool = root / "src/dotnet/Orbyss.Localization.Tool/LocalizationManagementTools.cs"
    source = tool.read_text(encoding="utf-8")
    names = re.findall(r'McpServerTool\(Name = "([^"]+)"', source)
    if len(names) != 16 or len(set(names)) != 16:
        raise AssertionError(f"Unexpected Localization MCP tool catalog: {names}")
    if "ClaimsPrincipal" not in source or "requestedAt" not in source:
        raise AssertionError("Localization tools do not preserve trusted identity and exact replay inputs")

    mcp_project = root / "src/dotnet/Orbyss.Localization.Mcp.AspNetCore/Orbyss.Localization.Mcp.AspNetCore.csproj"
    expected_packages = {
        "CShells.AspNetCore.Abstractions",
        "ModelContextProtocol.AspNetCore",
        "Orbyss.Foundation.Mcp.AspNetCore",
    }
    if references(mcp_project, "PackageReference") != expected_packages:
        raise AssertionError("Localization MCP package boundary changed")
    if references(mcp_project, "ProjectReference") != {
        "../Orbyss.Localization.Tool/Orbyss.Localization.Tool.csproj"
    }:
        raise AssertionError("Localization MCP contributor graph changed")

    feature = (root / "src/dotnet/Orbyss.Localization.Mcp.AspNetCore/OrbyssLocalizationMcpFeature.cs").read_text(encoding="utf-8")
    for required in ("IShellFeature", "DependsOn", "WithTools<LocalizationManagementTools>"):
        if required not in feature:
            raise AssertionError(f"Localization MCP contributor is missing {required}")
    for forbidden in ("IWebShellFeature", "MapMcp", "WithToolsFromAssembly", "AllowAnonymous"):
        if forbidden in feature:
            raise AssertionError(f"Localization MCP contributor contains unsafe transport behavior: {forbidden}")

    probe = root / "tests/dotnet/Orbyss.Localization.Management.Probe/Orbyss.Localization.Management.Probe.csproj"
    result = subprocess.run(
        ["dotnet", "run", "--project", str(probe), "--configuration", "Release", "--no-build"],
        cwd=root,
        capture_output=True,
        text=True,
        timeout=180,
        check=False,
    )
    if result.returncode:
        raise AssertionError(f"Localization management probe failed.\nstdout:\n{result.stdout}\nstderr:\n{result.stderr}")
    print("Orbyss Localization governed MCP management validation passed.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
