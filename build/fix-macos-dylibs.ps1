param(
    [Parameter(Mandatory = $true)]
    [string]$NativeDir
)

$ErrorActionPreference = "Stop"

function Get-ToolPath([string]$name) {
    $cmd = Get-Command $name -ErrorAction SilentlyContinue
    if (-not $cmd) {
        Write-Warning "Missing required tool '$name'."
        return $null
    }
    return $cmd.Source
}

$otool = Get-ToolPath "otool"
$installNameTool = Get-ToolPath "install_name_tool"
if (-not $otool -or -not $installNameTool) {
    Write-Warning "otool or install_name_tool not found; skipping dylib fix."
    return
}

if (-not (Test-Path $NativeDir)) {
    throw "Native directory not found: $NativeDir"
}
$NativeDir = (Resolve-Path $NativeDir).Path

function Get-DylibDependencies([string]$path) {
    $lines = & $otool -L $path 2>$null
    if (-not $lines) {
        return @()
    }

    return $lines | Select-Object -Skip 1 | ForEach-Object {
        $trimmed = $_.Trim()
        if ($trimmed) {
            ($trimmed -split "\s+")[0]
        }
    }
}

function Copy-Dependency([string]$depPath) {
    $base = Split-Path $depPath -Leaf
    $dest = Join-Path $NativeDir $base
    if (Test-Path $dest) {
        return $dest
    }
    if (Test-Path $depPath) {
        Copy-Item $depPath $dest -Force
        return $dest
    }

    Write-Warning "Missing dependency: $depPath"
    return $null
}

$localDylibs = Get-ChildItem -Path $NativeDir -Filter "*.dylib" -File
$depsToCopy = New-Object System.Collections.Generic.HashSet[string]([StringComparer]::OrdinalIgnoreCase)
foreach ($lib in $localDylibs) {
    foreach ($dep in (Get-DylibDependencies $lib.FullName)) {
        if ($dep.StartsWith("@") -or $dep.StartsWith("/usr/lib") -or $dep.StartsWith("/System/")) {
            continue
        }
        [void]$depsToCopy.Add($dep)
    }
}

foreach ($dep in $depsToCopy) {
    [void](Copy-Dependency $dep)
}

$localDylibs = Get-ChildItem -Path $NativeDir -Filter "*.dylib" -File
$localNames = $localDylibs.Name

foreach ($lib in $localDylibs) {
    $base = $lib.Name
    try {
        & $installNameTool -id "@rpath/$base" $lib.FullName | Out-Null
    } catch {
        # best effort
    }

    try {
        & $installNameTool -add_rpath "@loader_path" $lib.FullName | Out-Null
    } catch {
        # best effort
    }

    $deps = Get-DylibDependencies $lib.FullName
    foreach ($dep in $deps) {
        $depBase = Split-Path $dep -Leaf
        if ($localNames -contains $depBase) {
            try {
                & $installNameTool -change $dep "@rpath/$depBase" $lib.FullName | Out-Null
            } catch {
                # best effort
            }
        }
    }
}

Write-Host "[macos] Dylib install names and rpaths updated in $NativeDir"
