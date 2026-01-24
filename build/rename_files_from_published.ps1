# ===========================
# Shotora Artifact Collector
# ===========================

$ErrorActionPreference = "SilentlyContinue"

# Configuration
$osList   = @("win", "linux", "velopack-win", "velopack-linux")
$archList = @("x86", "x64", "arm64")

$appName = "Shotora"

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path

function Get-OsLabel($os)
{
    if ($os -like "velopack-*") {
        $os = $os -replace "velopack-", ""
    }

    switch ($os) {
        "win"   { return "Windows" }
        "linux" { return "Linux" }
        default { return $null }
    }
}

foreach ($os in $osList)
{
    foreach ($arch in $archList)
    {
        $folderName = "$os-$arch"
        $sourceDir  = Join-Path $scriptDir $folderName

        if (-not (Test-Path $sourceDir)) {
            continue
        }

        $file = Get-ChildItem $sourceDir -File |
                Where-Object { $_.Extension -in ".exe", ".app", ".pkg" } |
                Select-Object -First 1

        if (-not $file) {
            continue
        }

        $isInstaller = $os -like "velopack-*"
        $typeLabel   = if ($isInstaller) { "Installer" } else { "Portable" }
        $osLabel     = Get-OsLabel $os
        $archLabel   = $arch  # keep lowercase

        if (-not $osLabel) {
            continue
        }

        $targetName = "{0}_{1}_{2}_{3}{4}" -f `
            $appName,
            $typeLabel,
            $osLabel,
            $archLabel,
            $file.Extension

        $targetPath = Join-Path $scriptDir $targetName

        Copy-Item $file.FullName $targetPath -Force
    }
}
