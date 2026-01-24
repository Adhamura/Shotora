# Repack Python 7z Archive Script
# This script repacks python.7z archives using LZMA2 compression WITHOUT the ARM64 filter
# so that older 7zz binaries (v21.07) can extract them.
#
# Usage: .\repack-python-7z.ps1 [-ArchivePath <path>]

param(
    [string]$ArchivePath = "D:\gitget\Shotora\Shotora.App.Services\PythonEmbed\Linux\arm64\python.7z"
)

$7zExe = "C:\Program Files\7-Zip\7z.exe"

# Check if 7-Zip exists
if (-not (Test-Path $7zExe)) {
    Write-Error "7-Zip not found at $7zExe. Please install 7-Zip or update the path."
    exit 1
}

# Check if archive exists
if (-not (Test-Path $ArchivePath)) {
    Write-Error "Archive not found: $ArchivePath"
    exit 1
}

$archiveDir = Split-Path -Parent $ArchivePath
$archiveName = Split-Path -Leaf $ArchivePath
$tempDir = Join-Path $archiveDir "temp_repack_$(Get-Random)"
$backupPath = "$ArchivePath.backup"

Write-Host "=== Python 7z Repack Script ===" -ForegroundColor Cyan
Write-Host "Archive: $ArchivePath"
Write-Host "Temp dir: $tempDir"
Write-Host ""

try {
    # Step 1: Create backup
    Write-Host "[1/4] Creating backup..." -ForegroundColor Yellow
    Copy-Item $ArchivePath $backupPath -Force
    Write-Host "       Backup created: $backupPath" -ForegroundColor Green

    # Step 2: Extract archive
    Write-Host "[2/4] Extracting archive..." -ForegroundColor Yellow
    New-Item -ItemType Directory -Path $tempDir -Force | Out-Null
    & $7zExe x $ArchivePath -o"$tempDir" -y
    if ($LASTEXITCODE -ne 0) {
        throw "Extraction failed with exit code $LASTEXITCODE"
    }
    Write-Host "       Extraction complete" -ForegroundColor Green

    # Step 3: Delete original and recompress with compatible settings
    Write-Host "[3/4] Recompressing with LZMA2 (no ARM64 filter)..." -ForegroundColor Yellow
    Remove-Item $ArchivePath -Force

    # Key flags:
    # -t7z      : 7z format
    # -m0=LZMA2 : Use LZMA2 compression
    # -mx=7     : Compression level 7 (good balance of size/speed)
    # -ms=on    : Solid archive (better compression)
    # -mf=off   : DISABLE executable filters (ARM64, BCJ, etc.) - THIS IS THE KEY!
    & $7zExe a -t7z -m0=LZMA2 -mx=7 -ms=on -mf=off $ArchivePath "$tempDir\*"
    if ($LASTEXITCODE -ne 0) {
        throw "Compression failed with exit code $LASTEXITCODE"
    }
    Write-Host "       Recompression complete" -ForegroundColor Green

    # Step 4: Cleanup
    Write-Host "[4/4] Cleaning up..." -ForegroundColor Yellow
    Remove-Item -Path $tempDir -Recurse -Force
    Remove-Item $backupPath -Force
    Write-Host "       Cleanup complete" -ForegroundColor Green

    # Show result
    $newSize = (Get-Item $ArchivePath).Length
    Write-Host ""
    Write-Host "=== SUCCESS ===" -ForegroundColor Green
    Write-Host "New archive size: $([math]::Round($newSize / 1MB, 2)) MB"
    Write-Host ""
    Write-Host "The archive is now compatible with 7zz v21.07 (ARM64 Linux)" -ForegroundColor Cyan

} catch {
    Write-Error "Error: $_"

    # Restore backup if exists
    if (Test-Path $backupPath) {
        Write-Host "Restoring backup..." -ForegroundColor Yellow
        Copy-Item $backupPath $ArchivePath -Force
        Remove-Item $backupPath -Force
    }

    # Cleanup temp dir if exists
    if (Test-Path $tempDir) {
        Remove-Item -Path $tempDir -Recurse -Force -ErrorAction SilentlyContinue
    }

    exit 1
}