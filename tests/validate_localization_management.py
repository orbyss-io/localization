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
    management_tool = root / "src/Orbyss.Localization.Management.Tool/LocalizationManagementTools.cs"
    management_source = management_tool.read_text(encoding="utf-8")
    management_names = re.findall(r'McpServerTool\(Name = "([^"]+)"', management_source)
    if len(management_names) != 14 or len(set(management_names)) != 14:
        raise AssertionError(f"Unexpected Localization management MCP tool catalog: {management_names}")
    if "ClaimsPrincipal" not in management_source or "requestedAt" not in management_source:
        raise AssertionError("Localization tools do not preserve trusted identity and exact replay inputs")
    runtime_source = (root / "src/Orbyss.Localization.Runtime.Tool/LocalizationRuntimeTools.cs").read_text(encoding="utf-8")
    runtime_names = re.findall(r'McpServerTool\(Name = "([^"]+)"', runtime_source)
    if runtime_names != ["localization.runtime.resolve", "localization.runtime.bundle"]:
        raise AssertionError(f"Unexpected Localization runtime MCP tool catalog: {runtime_names}")
    if any("runtime" in name for name in management_names) or set(management_names) & set(runtime_names):
        raise AssertionError("Localization management and runtime MCP capabilities overlap")

    mcp_project = root / "src/Orbyss.Localization.Management.Mcp.AspNetCore/Orbyss.Localization.Management.Mcp.AspNetCore.csproj"
    expected_packages = {
        "CShells.AspNetCore.Abstractions",
        "ModelContextProtocol.AspNetCore",
        "Orbyss.Foundation.Mcp.AspNetCore",
    }
    if references(mcp_project, "PackageReference") != expected_packages:
        raise AssertionError("Localization MCP package boundary changed")
    if references(mcp_project, "ProjectReference") != {
        "../Orbyss.Localization.Management/Orbyss.Localization.Management.csproj",
        "../Orbyss.Localization.Management.Tool/Orbyss.Localization.Management.Tool.csproj",
    }:
        raise AssertionError("Localization management MCP contributor graph changed")

    feature = (root / "src/Orbyss.Localization.Management.Mcp.AspNetCore/OrbyssLocalizationManagementMcpFeature.cs").read_text(encoding="utf-8")
    for required in ("IShellFeature", "FoundationMcpFeature", "OrbyssLocalizationManagementFeature", "WithTools<LocalizationManagementTools>"):
        if required not in feature:
            raise AssertionError(f"Localization MCP contributor is missing {required}")
    for forbidden in ("IWebShellFeature", "MapMcp", "WithToolsFromAssembly", "AllowAnonymous"):
        if forbidden in feature:
            raise AssertionError(f"Localization MCP contributor contains unsafe transport behavior: {forbidden}")

    runtime_mcp_project = root / "src/Orbyss.Localization.Runtime.Mcp.AspNetCore/Orbyss.Localization.Runtime.Mcp.AspNetCore.csproj"
    if references(runtime_mcp_project, "PackageReference") != expected_packages:
        raise AssertionError("Localization runtime MCP package boundary changed")
    if references(runtime_mcp_project, "ProjectReference") != {
        "../Orbyss.Localization.Runtime/Orbyss.Localization.Runtime.csproj",
        "../Orbyss.Localization.Runtime.Tool/Orbyss.Localization.Runtime.Tool.csproj",
    }:
        raise AssertionError("Localization runtime MCP contributor graph changed")
    runtime_feature = (root / "src/Orbyss.Localization.Runtime.Mcp.AspNetCore/OrbyssLocalizationRuntimeMcpFeature.cs").read_text(encoding="utf-8")
    for required in ("IShellFeature", "FoundationMcpFeature", "OrbyssLocalizationRuntimeFeature", "WithTools<LocalizationRuntimeTools>"):
        if required not in runtime_feature:
            raise AssertionError(f"Localization runtime MCP contributor is missing {required}")

    probe = root / "tests/Orbyss.Localization.Management.Probe/Orbyss.Localization.Management.Probe.csproj"
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
    print("Orbyss Localization governed split management/runtime MCP validation passed.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
