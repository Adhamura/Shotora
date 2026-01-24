param(
    [Parameter(Mandatory = $true)][string]$PackDir,
    [Parameter(Mandatory = $true)][string]$OutputDir,
    [Parameter(Mandatory = $true)][string]$Runtime,
    [string]$Channel = "stable",
    [string]$Version,
    [string]$MainExe = ""
)

$ErrorActionPreference = "Stop"

$repoRoot   = Resolve-Path (Join-Path $PSScriptRoot "..")
$publishDir = [System.IO.Path]::GetFullPath($PackDir)
if (-not (Test-Path $publishDir)) { throw "PackDir not found: $publishDir" }
$releaseDir = if ([string]::IsNullOrWhiteSpace($OutputDir)) {
    [System.IO.Path]::GetFullPath((Join-Path $repoRoot "bin/velopack/$Runtime"))
} else {
    [System.IO.Path]::GetFullPath($OutputDir)
}
if (-not (Test-Path $releaseDir)) { New-Item -ItemType Directory -Path $releaseDir | Out-Null }

Write-Host "Restoring tools..."
dotnet tool restore | Out-Null
Write-Host "Packing using existing directory: $publishDir"

if (-not $Version) {
    $dll = Join-Path $publishDir "Shotora.App.dll"
    if (Test-Path $dll) {
        $fileVersion = (Get-Item $dll).VersionInfo.ProductVersion
        $parsedVersion = $null
        if ([System.Version]::TryParse($fileVersion, [ref]$parsedVersion)) {
            $Version = $parsedVersion.ToString(3)
        }
    } else {
        $exeProbe = Join-Path $publishDir "Shotora.App.exe"
        if (Test-Path $exeProbe) {
            $fileVersion = (Get-Item $exeProbe).VersionInfo.ProductVersion
            $parsedVersion = $null
            if ([System.Version]::TryParse($fileVersion, [ref]$parsedVersion)) {
                $Version = $parsedVersion.ToString(3)
            }
        }
    }

    if (-not $Version) {
        $Version = "1.0.0"
    }
}

$icon         = Join-Path $repoRoot "Shotora.App/Assets/Shotora.ico"
$splash       = Join-Path $repoRoot "build/Installer/splash.png"
$installerDir = Join-Path $repoRoot "build/Installer"
$pkgArgNames  = @("--pkgWelcome", "--pkgReadme", "--pkgLicense", "--pkgConclusion")
$pkgDocs = @{
    "--pkgWelcome"    = Join-Path $installerDir "Welcome.txt"
    "--pkgReadme"     = Join-Path $installerDir "Readme.txt"
    "--pkgLicense"    = Join-Path $installerDir "License.txt"
    "--pkgConclusion" = Join-Path $installerDir "Conclusion.txt"
}

$env:DOTNET_ROLL_FORWARD = "LatestMajor"

# Detect optional pkg page support only if we can run vpk with roll-forward.
$pkgPagesSupported = $false
$helpText = $null
try {
    $helpText = (dotnet tool run vpk -- pack --help 2>&1) -join "`n"
    $matchedCount = ($pkgArgNames | Where-Object { $helpText -match [Regex]::Escape($_) }).Count
    if ($matchedCount -eq $pkgArgNames.Count) {
        $pkgPagesSupported = $true
    }
} catch {
    Write-Warning "Unable to probe vpk for pkg page support; skipping optional installer pages. Error: $_"
}

# Detect whether this vpk supports cross-platform pack subcommands.
$linuxPackSupported = $false
if ($helpText) {
    $linuxPackSupported = $helpText -match "vpk\s+linux\b"
}

# Choose the correct vpk OS directive and main executable name based on runtime.
$packCommand = @("pack")
$mainExe     = if ([string]::IsNullOrWhiteSpace($MainExe)) { "Shotora.App.exe" } else { $MainExe }
if ($Runtime -like "linux*") {
    if ($linuxPackSupported) {
        $packCommand = @("linux", "pack")
        if ([string]::IsNullOrWhiteSpace($MainExe)) { $mainExe = "Shotora.App" }
    } else {
        throw "This vpk version does not support 'vpk linux pack'. Update the vpk tool or build the Linux package on a Linux machine."
    }
} elseif ($Runtime -like "osx*") {
    $packCommand = @("mac", "pack")
    if ([string]::IsNullOrWhiteSpace($MainExe)) { $mainExe = "Shotora.App" }
}
if ($Runtime -like "win*" -and -not $mainExe.EndsWith(".exe")) { $mainExe = "$mainExe.exe" }

Write-Host "Packing Velopack release $Version ($Runtime, channel=$Channel)..."
& {
    $argsList = @(
        "--packId", "Shotora",
        "--packVersion", $Version,
        "--packDir", $publishDir,
        "--outputDir", $releaseDir,
        "--runtime", $Runtime,
        "--channel", $Channel,
        "--packTitle", "Shotora",
        "--packAuthors", "Kazuki Kimura",
        "--mainExe", $mainExe,
        "--icon", $icon,
        "--splashImage", $splash,
        "--shortcuts", "Desktop,StartMenuRoot"
    )

    if ($pkgPagesSupported) {
        foreach ($entry in $pkgDocs.GetEnumerator()) {
            if (Test-Path $entry.Value) {
                $argsList += @($entry.Key, $entry.Value)
            } else {
                Write-Warning "Optional installer page missing: $($entry.Value)"
            }
        }
    } else {
        Write-Warning "This vpk version does not support pkg welcome/readme/license/conclusion pages. Update vpk if you need them."
    }

    $fullCmd = @($packCommand + $argsList)
    Write-Host "Running: vpk $($fullCmd -join ' ')" -ForegroundColor Cyan
    dotnet tool run vpk -- @fullCmd
}

Write-Host ""
Write-Host "Done."
Write-Host "Releases: $releaseDir"
Write-Host "Installer artifacts will be inside that folder."