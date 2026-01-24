param(
    [Parameter(Mandatory = $true)][string]$PackDir,
    [Parameter(Mandatory = $true)][string]$OutputDir,
    [Parameter(Mandatory = $true)][string]$Runtime,
    [string]$Channel = "mac",
    [string]$Version,
    [string]$MainExe = "Shotora.App"
)

$ErrorActionPreference = "Stop"

$isMac = [System.Runtime.InteropServices.RuntimeInformation]::IsOSPlatform([System.Runtime.InteropServices.OSPlatform]::OSX)
if (-not $isMac) {
    throw "macOS packages must be built on macOS (Velopack uses macOS tooling like hdiutil). Run this script on a Mac."
}

$repoRoot   = Resolve-Path (Join-Path $PSScriptRoot "..")
$releaseDir = [System.IO.Path]::GetFullPath($OutputDir)
if (-not (Test-Path $releaseDir)) { New-Item -ItemType Directory -Path $releaseDir | Out-Null }
$publishDir = [System.IO.Path]::GetFullPath($PackDir)
if (-not (Test-Path $publishDir)) { throw "PackDir not found: $publishDir" }
$mainExe    = $MainExe
$needsExe   = $Runtime -like "win*"
if ($needsExe -and -not $mainExe.EndsWith(".exe")) { $mainExe = "$mainExe.exe" }

Write-Host "Restoring tools..."
dotnet tool restore | Out-Null
Write-Host "Packing using existing directory: $publishDir"

# Fix macOS dylib install names so bundled libs resolve locally
$arch = "x64"
if ($Runtime -like "*arm64*") {
    $arch = "arm64"
}
$nativeDir = Join-Path $publishDir "Binaries/Mac/$arch"
$fixScript = Join-Path $repoRoot "build/fix-macos-dylibs.ps1"
if ((Test-Path $fixScript) -and (Test-Path $nativeDir)) {
    Write-Host "Fixing macOS dylib install names in $nativeDir"
    pwsh -NoProfile -ExecutionPolicy Bypass -File $fixScript -NativeDir $nativeDir
} else {
    Write-Warning "Skipping dylib fix: missing $fixScript or $nativeDir"
}

# Ensure main executable name matches platform expectations
$publishedHost = Join-Path $publishDir "Shotora.App"
$renamedHost   = Join-Path $publishDir $mainExe
if ($mainExe -ne "Shotora.App" -and (Test-Path $publishedHost)) {
    if (Test-Path $renamedHost) { Remove-Item $renamedHost -Force }
    Rename-Item $publishedHost $renamedHost
}
if (-not (Test-Path $renamedHost)) {
    throw "Expected host at $renamedHost was not created. Ensure your publish output contains $mainExe (or Shotora.App to be renamed)."
}

if (-not $Version) {
    $dll = Join-Path $publishDir "Shotora.App.dll"
    if (Test-Path $dll) {
        $fileVersion = (Get-Item $dll).VersionInfo.ProductVersion
        $parsedVersion = $null
        if ([System.Version]::TryParse($fileVersion, [ref]$parsedVersion)) {
            $Version = $parsedVersion.ToString(3)
        }
    } else {
        $exeProbe = Join-Path $publishDir $mainExe
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

$iconIcns = Join-Path $repoRoot "Shotora.App/Assets/Shotora.icns"
$installerDir = Join-Path $repoRoot "build/Installer"
$pkgArgNames  = @("--pkgWelcome", "--pkgReadme", "--pkgLicense", "--pkgConclusion")
$pkgDocs = @{
    "--pkgWelcome"    = Join-Path $installerDir "Welcome.txt"
    "--pkgReadme"     = Join-Path $installerDir "Readme.txt"
    "--pkgLicense"    = Join-Path $installerDir "License.txt"
    "--pkgConclusion" = Join-Path $installerDir "Conclusion.txt"
}

$env:DOTNET_ROLL_FORWARD = "LatestMajor"

$pkgPagesSupported = $false
$helpText = $null
try {
    $helpText = (dotnet tool run vpk -- pack --help 2>&1) -join "`n"
    $matchedCount = ($pkgArgNames | Where-Object { $helpText -match [Regex]::Escape($_) }).Count
    if ($matchedCount -eq $pkgArgNames.Count) { $pkgPagesSupported = $true }
} catch {
    Write-Warning "Unable to probe vpk for pkg page support; skipping optional installer pages. Error: $_"
}

$macPackSupported = $false
$linuxPackSupported = $false
if ($helpText) {
    $macPackSupported = $helpText -match "vpk\s+mac\b"
    $linuxPackSupported = $helpText -match "vpk\s+linux\b"
}

Write-Host "Packing Velopack release $Version ($Runtime, channel=$Channel)..."
Write-Host "Using main executable: $mainExe"
& {
    $packCommand = @("pack")
    if ($Runtime -like "linux*") {
        if ($linuxPackSupported) {
            $packCommand = @("linux", "pack")
        } else {
            Write-Warning "This vpk version does not support 'vpk linux pack'; falling back to 'vpk pack'. Update vpk if you need the linux subcommand."
        }
    } elseif ($Runtime -like "osx*") {
        if ($macPackSupported) {
            $packCommand = @("mac", "pack")
        } else {
            Write-Warning "This vpk version does not support 'vpk mac pack'; falling back to 'vpk pack'. Update vpk if you need the mac subcommand."
        }
    }

    $argsList = @(
        "--packId", "Shotora",
        "--packVersion", $Version,
        "--packDir", $publishDir,
        "--outputDir", $releaseDir,
        "--runtime", $Runtime,
        "--channel", $Channel,
        "--packTitle", "Shotora",
        "--packAuthors", "Shotora",
        "--mainExe", $mainExe
    )
    if (Test-Path $iconIcns) {
        $argsList += @("--icon", $iconIcns)
    } else {
        Write-Warning "No .icns icon found at $iconIcns; proceeding without --icon."
    }

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
if ($LASTEXITCODE -ne 0) {
    throw "vpk pack failed with exit code $LASTEXITCODE"
}

Write-Host ""
Write-Host "Done."
Write-Host "Releases: $releaseDir"
Write-Host "Installer artifacts will be inside that folder."