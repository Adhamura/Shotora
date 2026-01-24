param(
    [Parameter(Mandatory = $true)][string]$PackDir,
    [Parameter(Mandatory = $true)][string]$Runtime,
    [string]$OutputDir = "",
    [string]$Channel = "stable",
    [string]$Version = "",
    [string]$MainExe = "",
    [string]$PackId = "Shotora",
    [string]$PackTitle = "Shotora",
    [string]$PackAuthors = "Kazuki Kimura",
    [switch]$NoInstaller
)

$ErrorActionPreference = "Stop"

# Repo root (sibling of build folder)
$repoRoot = Resolve-Path (Join-Path $PSScriptRoot "..")

# Resolve and validate input/output
$packDirFull = [System.IO.Path]::GetFullPath($PackDir)
if (-not (Test-Path $packDirFull))
{
    throw "PackDir not found: $packDirFull"
}

$outputDirFull = if ( [string]::IsNullOrWhiteSpace($OutputDir))
{
    # Default to a sibling directory to avoid recursive copies (vpk copies packDir into OutputDir)
    $parent = Split-Path $packDirFull -Parent
    Join-Path $parent ("velopack-" + $Runtime)
}
else
{
    [System.IO.Path]::GetFullPath($OutputDir)
}

# Prevent OutputDir being inside PackDir (causes recursive copies on mac/linux)
$isSubPath = $outputDirFull.StartsWith($packDirFull, [System.StringComparison]::OrdinalIgnoreCase)
if ($isSubPath)
{
    $parent = Split-Path $packDirFull -Parent
    $outputDirFull = Join-Path $parent ("velopack-" + $Runtime)
    Write-Warning "OutputDir was inside PackDir; using safe sibling instead: $outputDirFull"
}

if (-not (Test-Path $outputDirFull))
{
    New-Item -ItemType Directory -Path $outputDirFull | Out-Null
}

# Remove any prior velopack artifacts inside PackDir to avoid recursive copy issues on mac/linux
$nestedVpk = Join-Path $packDirFull "velopack"
if (Test-Path $nestedVpk)
{
    Write-Warning "Removing nested velopack folder inside PackDir to avoid recursive copy: $nestedVpk"
    Remove-Item -Recurse -Force $nestedVpk
}

Write-Host "Velopack (cake-based) packing..." -ForegroundColor Cyan
Write-Host "  PackDir   : $packDirFull"
Write-Host "  OutputDir : $outputDirFull"
Write-Host "  Runtime   : $Runtime"
Write-Host "  Channel   : $Channel"

# Restore tools only; never publish here.
dotnet tool restore | Out-Null

# Determine main exe
$mainExe = if (-not [string]::IsNullOrWhiteSpace($MainExe))
{
    $MainExe
}
elseif ($Runtime -like "win*")
{
    "Shotora.App.exe"
}
else
{
    "Shotora.App"
}
if ($Runtime -like "win*" -and -not $mainExe.EndsWith(".exe"))
{
    $mainExe = "$mainExe.exe"
}

# Derive version from dll/exe if not provided
if ( [string]::IsNullOrWhiteSpace($Version))
{
    $dllPath = Join-Path $packDirFull "Shotora.App.dll"
    if (Test-Path $dllPath)
    {
        $fileVersion = (Get-Item $dllPath).VersionInfo.ProductVersion
        $parsed = $null
        if ( [System.Version]::TryParse($fileVersion, [ref]$parsed))
        {
            $Version = $parsed.ToString(3)
        }
    }
    if ( [string]::IsNullOrWhiteSpace($Version))
    {
        $exePath = Join-Path $packDirFull $mainExe
        if (Test-Path $exePath)
        {
            $fileVersion = (Get-Item $exePath).VersionInfo.ProductVersion
            $parsed = $null
            if ( [System.Version]::TryParse($fileVersion, [ref]$parsed))
            {
                $Version = $parsed.ToString(3)
            }
        }
    }
    if ( [string]::IsNullOrWhiteSpace($Version))
    {
        $Version = "1.0.0"
    }
}

# Ensure vpk can roll forward to installed runtimes (vpk targets net9)
$env:DOTNET_ROLL_FORWARD = "LatestMajor"

# If targeting macOS, delegate to pack-macos.ps1 to build the .pkg (same behavior as the dedicated script).
if ($Runtime -like "osx*" -or $Runtime -like "mac*")
{
    $macScript = Join-Path $PSScriptRoot "pack-macos.ps1"
    Write-Host "Delegating macOS packaging to pack-macos.ps1..." -ForegroundColor Cyan
    $macArgs = @(
        "-File", $macScript,
        "-Runtime", $Runtime,
        "-Channel", $Channel,
        "-Version", $Version,
        "-PackDir", $packDirFull,
        "-OutputDir", $outputDirFull,
        "-MainExe", $mainExe
    )
    pwsh -NoProfile -ExecutionPolicy Bypass @macArgs
    if ($LASTEXITCODE -ne 0)
    {
        throw "pack-macos.ps1 failed with exit code $LASTEXITCODE"
    }
    Write-Host "Done. Velopack artifacts: $outputDirFull"
    return
}

# Detect optional linux/mac subcommands (for non-mac runtimes)
$helpText = (dotnet tool run vpk -- pack --help 2>&1) -join "`n"
$supportsLinux = $helpText -match "vpk\s+linux\b"
$supportsMac = $helpText -match "vpk\s+mac\b"

# Build vpk command
$packCmd = @("pack")
if ($Runtime -like "linux*" -and $supportsLinux)
{
    $packCmd = @("linux", "pack")
}
if ($Runtime -like "osx*" -and $supportsMac)
{
    $packCmd = @("mac", "pack")
}

$icon = Join-Path $repoRoot "Shotora.App/Assets/Shotora.ico"
$splash = Join-Path $repoRoot "build/Installer/splash.png"

$args = @(
    $packCmd +
            @("--packId", $PackId,
            "--packVersion", $Version,
            "--packDir", $packDirFull,
            "--outputDir", $outputDirFull,
            "--runtime", $Runtime,
            "--channel", $Channel,
            "--packTitle", $PackTitle,
            "--packAuthors", $PackAuthors,
            "--mainExe", $mainExe,
            "--exclude", ".*velopack.*|.*\.DS_Store")
)

if (Test-Path $icon)   { $args += @("--icon", $icon) } else { Write-Warning "Icon missing at $icon; skipping --icon" }
if (Test-Path $splash) { $args += @("--splashImage", $splash) } else { Write-Warning "Splash image missing at $splash; skipping --splashImage" }

if ($NoInstaller)
{
    $args += "--noInst"
}

Write-Host "Running: vpk $( $args -join ' ' )" -ForegroundColor Cyan
& dotnet tool run vpk -- @args
if ($LASTEXITCODE -ne 0)
{
    throw "vpk pack failed with exit code $LASTEXITCODE"
}

Write-Host ""
Write-Host "Done."
Write-Host "Velopack artifacts: $outputDirFull"