<#
.SYNOPSIS
    Generates detailed file-level Markdown coverage report with interactive folder tree.

.DESCRIPTION
    Analyzes DotCover DetailedXml format and produces comprehensive Markdown documentation
    with hierarchical folder structure, file-level coverage, badges, and visual indicators.

.PARAMETER CoverageFile
    Path to the DotCover DetailedXml coverage file. Defaults to './snapshot1.xml'

.PARAMETER OutputFile
    Path to the output Markdown file. Defaults to './COVERAGE-DETAILED.md'

.PARAMETER MinimumCoverage
    Minimum coverage threshold percentage. Defaults to 80.

.PARAMETER ShowFullPaths
    Show full file paths instead of relative paths. Defaults to $false.

.PARAMETER GenerateBadges
    Generate SVG-style badge links. Defaults to $true.

.PARAMETER SolutionPath
    Base path to strip from file paths (e.g., solution folder). Auto-detected if not provided.

.EXAMPLE
    .\generate-detailed-coverage.ps1

.EXAMPLE
    .\generate-detailed-coverage.ps1 -MinimumCoverage 90

.EXAMPLE
    .\generate-detailed-coverage.ps1 -SolutionPath "\Shotora"

.NOTES
    Author: Shotora Team
    Version: 1.0.0
    Requires: PowerShell 5.1+
#>

[CmdletBinding()]
param(
    [Parameter(Mandatory=$false)]
    [string]$CoverageFile = ".\snapshot1.xml",
    
    [Parameter(Mandatory=$false)]
    [string]$OutputFile = ".\COVERAGE-DETAILED.md",
    
    [Parameter(Mandatory=$false)]
    [ValidateRange(0, 100)]
    [int]$MinimumCoverage = 80,
    
    [Parameter(Mandatory=$false)]
    [bool]$ShowFullPaths = $false,
    
    [Parameter(Mandatory=$false)]
    [bool]$GenerateBadges = $true,
    
    [Parameter(Mandatory=$false)]
    [string]$SolutionPath = ""
)

#region Helper Functions

function Get-CoverageIcon {
    param([int]$Coverage)
    
    if ($Coverage -eq 100) { return "" }
    elseif ($Coverage -ge 90) { return "" }
    elseif ($Coverage -ge 80) { return "" }
    elseif ($Coverage -ge 60) { return "" }
    elseif ($Coverage -eq 0) { return "" }
    else { return "[!]" }
}

function Get-CoverageLabel {
    param([int]$Coverage)
    
    $color = "lightgrey"
    if ($Coverage -ge 90) { $color = "brightgreen" }
    elseif ($Coverage -ge 80) { $color = "green" }
    elseif ($Coverage -ge 70) { $color = "yellowgreen" }
    elseif ($Coverage -ge 60) { $color = "yellow" }
    elseif ($Coverage -ge 40) { $color = "orange" }
    elseif ($Coverage -gt 0) { $color = "red" }
    
    return "![${Coverage}%](https://img.shields.io/badge/${Coverage}%25-${color})"
}

function Get-ProgressBar {
    param(
        [int]$Coverage,
        [int]$Width = 20
    )
    
    $filled = [Math]::Floor($Coverage / 100.0 * $Width)
    $empty = $Width - $filled
    
    $bar = ("#" * $filled) + ("-" * $empty)
    return "``[$bar]`` $Coverage%"
}

function Format-Number {
    param([int]$Number)
    return $Number.ToString("N0")
}

function Get-RelativePath {
    param(
        [string]$Path,
        [string]$BasePath
    )
    
    if ([string]::IsNullOrEmpty($Path)) { return "" }
    
    $Path = $Path.Replace('/', '\')
    $BasePath = $BasePath.Replace('/', '\')
    
    if ($Path.StartsWith($BasePath, [StringComparison]::OrdinalIgnoreCase)) {
        return $Path.Substring($BasePath.Length).TrimStart('\')
    }
    
    return $Path
}

function Build-FolderTree {
    param(
        [System.Collections.Generic.List[PSObject]]$Files,
        [string]$BasePath
    )
    
    $tree = @{}
    
    foreach ($file in $Files) {
        $relativePath = Get-RelativePath -Path $file.Path -BasePath $BasePath
        
        # Skip if no relative path (shouldn't happen, but safety check)
        if ([string]::IsNullOrEmpty($relativePath)) { continue }
        
        $parts = $relativePath.Split('\') | Where-Object { -not [string]::IsNullOrWhiteSpace($_) }
        
        # Skip drive letters and absolute path markers
        if ($parts.Count -eq 0) { continue }
        
        $currentLevel = $tree
        for ($i = 0; $i -lt $parts.Length; $i++) {
            $part = $parts[$i]
            
            # Skip empty parts
            if ([string]::IsNullOrWhiteSpace($part)) { continue }
            
            if ($i -eq $parts.Length - 1) {
                # Leaf node (file)
                if (-not $currentLevel.ContainsKey('_files_')) {
                    $currentLevel['_files_'] = @()
                }
                $currentLevel['_files_'] += $file
            } else {
                # Directory node - first level should be project/assembly names
                if (-not $currentLevel.ContainsKey($part)) {
                    $currentLevel[$part] = @{}
                }
                $currentLevel = $currentLevel[$part]
            }
        }
    }
    
    return $tree
}

function Get-FolderStats {
    param([hashtable]$Tree)
    
    $total = 0
    $covered = 0
    
    if ($Tree.ContainsKey('_files_')) {
        foreach ($file in $Tree['_files_']) {
            $total += $file.TotalStatements
            $covered += $file.CoveredStatements
        }
    }
    
    foreach ($key in $Tree.Keys) {
        if ($key -ne '_files_') {
            $subStats = Get-FolderStats -Tree $Tree[$key]
            $total += $subStats.Total
            $covered += $subStats.Covered
        }
    }
    
    return @{ Total = $total; Covered = $covered }
}

function Render-FolderTree {
    param(
        [hashtable]$Tree,
        [System.Text.StringBuilder]$Markdown,
        [string]$CurrentPath = "",
        [int]$MinCoverage,
        [int]$Level = 0
    )
    
    $folders = $Tree.Keys | Where-Object { $_ -ne '_files_' } | Sort-Object
    $files = if ($Tree.ContainsKey('_files_')) { $Tree['_files_'] | Sort-Object { $_.Name } } else { @() }
    
    # Determine if this is a project-level folder (first level)
    $isProjectLevel = ($Level -eq 0)
    
    foreach ($folder in $folders) {
        $folderPath = if ($CurrentPath) { "$CurrentPath\$folder" } else { $folder }
        $subStats = Get-FolderStats -Tree $Tree[$folder]
        $folderCoverage = if ($subStats.Total -gt 0) { [Math]::Round(($subStats.Covered / $subStats.Total) * 100) } else { 0 }
        
        $icon = Get-CoverageIcon -Coverage $folderCoverage
        $badge = Get-CoverageLabel -Coverage $folderCoverage
        $warning = if ($folderCoverage -lt $MinCoverage) { " **WARNING**" } else { "" }
        
        # Use different formatting for project level vs subfolders
        $headingLevel = if ($isProjectLevel) { "###" } else { "####" }
        
        # Use SVG badges instead of text labels
        if ($isProjectLevel) {
            $folderIcon = "![Project](https://img.shields.io/badge/Project-blue?style=for-the-badge&logo=nuget)"
        } else {
            $folderIcon = "![Folder](https://img.shields.io/badge/Folder-gray?style=for-the-badge&logo=data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHZpZXdCb3g9IjAgMCAyNCAyNCI+PHBhdGggZmlsbD0iI2ZmZmZmZiIgZD0iTTMgNiBDMyA0LjkgMy45IDQgNSA0IEgxMCBMMTIgNiBIMTkgQzIwLjEgNiAyMSA2LjkgMjEgOCBWMTggQzIxIDE5LjEgMjAuMSAyMCAxOSAyMCBINSBDMy45IDIwIDMgMTkuMSAzIDE4IFoiLz48L3N2Zz4=)"
        }
        
        [void]$Markdown.AppendLine("")
        [void]$Markdown.AppendLine("<details>")
        [void]$Markdown.AppendLine("<summary>")
        [void]$Markdown.AppendLine("")
        [void]$Markdown.AppendLine("$headingLevel $folderIcon **$folder** $badge")
        [void]$Markdown.AppendLine("")
        if ($folderCoverage -lt $MinCoverage) {
            [void]$Markdown.AppendLine("> ![Warning](https://img.shields.io/badge/WARNING-Coverage_below_threshold-red?style=flat-square) | **$(Format-Number $subStats.Covered)** / $(Format-Number $subStats.Total) statements covered")
        } else {
            [void]$Markdown.AppendLine("> **$(Format-Number $subStats.Covered)** / $(Format-Number $subStats.Total) statements covered")
        }
        [void]$Markdown.AppendLine("")
        [void]$Markdown.AppendLine("</summary>")
        [void]$Markdown.AppendLine("")
        
        # Render files FIRST (before subfolders) to keep them inside this details tag
        $currentFiles = if ($Tree[$folder].ContainsKey('_files_')) { $Tree[$folder]['_files_'] | Sort-Object { $_.Name } } else { @() }
        if ($currentFiles.Count -gt 0) {
            [void]$Markdown.AppendLine("| File | Coverage | Statements | Progress |")
            [void]$Markdown.AppendLine("|------|----------|------------|----------|")
            
            foreach ($file in $currentFiles) {
                $fileIcon = Get-CoverageIcon -Coverage $file.Coverage
                $fileName = $file.Name
                $fileBadge = Get-CoverageLabel -Coverage $file.Coverage
                $fileProgress = Get-ProgressBar -Coverage $file.Coverage -Width 15
                $fileWarning = if ($file.Coverage -lt $MinCoverage) { " **!**" } else { "" }
                
                [void]$Markdown.AppendLine("| $fileIcon ``$fileName`` | $fileBadge$fileWarning | $(Format-Number $file.CoveredStatements) / $(Format-Number $file.TotalStatements) | $fileProgress |")
            }
            
            [void]$Markdown.AppendLine("")
        }
        
        # Then render subfolders (nested details tags)
        Render-FolderTree -Tree $Tree[$folder] -Markdown $Markdown -CurrentPath $folderPath -MinCoverage $MinCoverage -Level ($Level + 1)
        
        [void]$Markdown.AppendLine("</details>")
        [void]$Markdown.AppendLine("")
    }
}

function Parse-Namespace {
    param($Namespace, $FileIndex, $FileDataList)
    
    if ($Namespace.Type) {
        foreach ($type in $Namespace.Type) {
            Parse-Type -Type $type -FileIndex $FileIndex -FileDataList $FileDataList
        }
    }
    
    if ($Namespace.Namespace) {
        foreach ($ns in $Namespace.Namespace) {
            Parse-Namespace -Namespace $ns -FileIndex $FileIndex -FileDataList $FileDataList
        }
    }
}

function Parse-Type {
    param($Type, $FileIndex, $FileDataList)
    
    $methods = @()
    if ($Type.Constructor) { $methods += @($Type.Constructor) }
    if ($Type.Method) { $methods += @($Type.Method) }
    if ($Type.Property) {
        foreach ($prop in $Type.Property) {
            if ($prop.PropertyGetter) { $methods += $prop.PropertyGetter }
            if ($prop.PropertySetter) { $methods += $prop.PropertySetter }
        }
    }
    
    foreach ($method in $methods) {
        if (-not $method) { continue }
        
        if ($method.Statement) {
            foreach ($stmt in $method.Statement) {
                $fileIdx = [int]$stmt.FileIndex
                if ($FileIndex.ContainsKey($fileIdx)) {
                    $script:fileDataList.Add([PSCustomObject]@{
                        FileIndex = $fileIdx
                        Path = $FileIndex[$fileIdx].Path
                        Name = $FileIndex[$fileIdx].Name
                        Covered = if ($stmt.Covered -eq "True") { 1 } else { 0 }
                    })
                }
            }
        }
    }
}

#endregion

#region Main Script

$ScriptDir = $PSScriptRoot
if ([string]::IsNullOrEmpty($ScriptDir)) {
    $ScriptDir = Get-Location
}

$CoverageFilePath = Join-Path $ScriptDir $CoverageFile
$OutputFilePath = Join-Path $ScriptDir $OutputFile

Write-Host "================================================================" -ForegroundColor Cyan
Write-Host "     DotCover Detailed Coverage Report Generator               " -ForegroundColor Cyan
Write-Host "================================================================" -ForegroundColor Cyan
Write-Host ""

if (-not (Test-Path $CoverageFilePath)) {
    Write-Host "[ERROR] Coverage file not found: $CoverageFilePath" -ForegroundColor Red
    exit 1
}

Write-Host "[LOAD] Loading detailed coverage data..." -ForegroundColor Yellow

try {
    [xml]$coverageXml = Get-Content $CoverageFilePath -Raw -Encoding UTF8
    $root = $coverageXml.Root
    
    $totalCovered = [int]$root.CoveredStatements
    $totalStatements = [int]$root.TotalStatements
    $overallCoverage = [int]$root.CoveragePercent
    $dotCoverVersion = $root.DotCoverVersion
    $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    
    Write-Host "[INFO] Parsing file indices..." -ForegroundColor Green
    
    $fileIndex = @{}
    foreach ($file in $root.FileIndices.File) {
        $idx = [int]$file.Index
        $fileIndex[$idx] = @{
            Path = $file.Name
            Name = [System.IO.Path]::GetFileName($file.Name)
        }
    }
    
    Write-Host "[INFO] Processing $($fileIndex.Count) files..." -ForegroundColor Green
    
} catch {
    Write-Host "[ERROR] Failed to parse coverage XML: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

Write-Host "[BUILD] Analyzing file coverage..." -ForegroundColor Yellow

$fileDataList = New-Object 'System.Collections.Generic.List[PSObject]'

foreach ($assembly in $root.Assembly) {
    foreach ($namespace in $assembly.Namespace) {
        Parse-Namespace -Namespace $namespace -FileIndex $fileIndex -FileDataList $fileDataList
    }
}

Write-Host "[ANALYZE] Aggregating coverage by file..." -ForegroundColor Yellow

$fileStats = @{}
foreach ($item in $fileDataList) {
    $path = $item.Path
    
    if (-not $fileStats.ContainsKey($path)) {
        $fileStats[$path] = @{
            Path = $path
            Name = $item.Name
            TotalStatements = 0
            CoveredStatements = 0
        }
    }
    
    $fileStats[$path].TotalStatements++
    if ($item.Covered -eq 1) {
        $fileStats[$path].CoveredStatements++
    }
}

$files = New-Object 'System.Collections.Generic.List[PSObject]'
foreach ($path in $fileStats.Keys) {
    $stat = $fileStats[$path]
    $coverage = if ($stat.TotalStatements -gt 0) { [Math]::Round(($stat.CoveredStatements / $stat.TotalStatements) * 100) } else { 0 }
    
    $files.Add([PSCustomObject]@{
        Path = $stat.Path
        Name = $stat.Name
        TotalStatements = $stat.TotalStatements
        CoveredStatements = $stat.CoveredStatements
        Coverage = $coverage
    })
}

Write-Host "[INFO] Analyzed $($files.Count) files with coverage data" -ForegroundColor Green

# Determine base path (solution folder)
$basePath = ""
if (-not [string]::IsNullOrEmpty($SolutionPath)) {
    $basePath = $SolutionPath
    Write-Host "[INFO] Using provided solution path: $basePath" -ForegroundColor Green
} elseif ($files.Count -gt 0) {
    # Auto-detect solution folder by finding common path
    $firstPath = $files[0].Path
    $pathSeparator = "\"
    $parts = $firstPath.Split($pathSeparator)
    
    # Try to find solution folder (folder containing multiple project folders)
    for ($i = 0; $i -lt $parts.Length; $i++) {
        $testPath = $parts[0..$i] -join $pathSeparator
        if ($testPath -match 'Shotora$') {
            $basePath = $testPath
            break
        }
    }
    
    # Fallback: use parent of first project folder
    if ([string]::IsNullOrEmpty($basePath) -and $parts.Length -gt 2) {
        $basePath = $parts[0..($parts.Length-3)] -join $pathSeparator
    }
    
    Write-Host "[INFO] Auto-detected solution path: $basePath" -ForegroundColor Green
}

Write-Host "[BUILD] Generating Markdown report..." -ForegroundColor Yellow

$markdown = New-Object System.Text.StringBuilder

[void]$markdown.AppendLine("# Detailed Coverage Report")
[void]$markdown.AppendLine("")
[void]$markdown.AppendLine("> **Generated:** $timestamp  ")
[void]$markdown.AppendLine("> **Tool:** DotCover $dotCoverVersion  ")
[void]$markdown.AppendLine("> **Report Type:** Detailed File-Level Analysis  ")
[void]$markdown.AppendLine("> **Minimum Threshold:** $MinimumCoverage%")
[void]$markdown.AppendLine("")

[void]$markdown.AppendLine("## Overall Summary")
[void]$markdown.AppendLine("")
[void]$markdown.AppendLine("| Metric | Value |")
[void]$markdown.AppendLine("|--------|-------|")
[void]$markdown.AppendLine("| **Total Files** | $(Format-Number $files.Count) |")
[void]$markdown.AppendLine("| **Total Statements** | $(Format-Number $totalStatements) |")
[void]$markdown.AppendLine("| **Covered Statements** | $(Format-Number $totalCovered) |")

# Add coverage badge in the table if badges are enabled
if ($GenerateBadges) {
    $coverageBadge = Get-CoverageLabel -Coverage $overallCoverage
    [void]$markdown.AppendLine("| **Coverage** | $coverageBadge |")
} else {
    [void]$markdown.AppendLine("| **Coverage** | **$overallCoverage%** |")
}

[void]$markdown.AppendLine("")

$perfect = ($files | Where-Object { $_.Coverage -eq 100 }).Count
$high = ($files | Where-Object { $_.Coverage -ge 80 -and $_.Coverage -lt 100 }).Count
$medium = ($files | Where-Object { $_.Coverage -ge 60 -and $_.Coverage -lt 80 }).Count
$low = ($files | Where-Object { $_.Coverage -gt 0 -and $_.Coverage -lt 60 }).Count
$zero = ($files | Where-Object { $_.Coverage -eq 0 }).Count

[void]$markdown.AppendLine("### Coverage Distribution")
[void]$markdown.AppendLine("")
[void]$markdown.AppendLine("| Category | Files | Percentage |")
[void]$markdown.AppendLine("|----------|-------|------------|")
[void]$markdown.AppendLine("| **Perfect** (100%) | $perfect | $([Math]::Round($perfect / $files.Count * 100, 1))% |")
[void]$markdown.AppendLine("| **High** (80-99%) | $high | $([Math]::Round($high / $files.Count * 100, 1))% |")
[void]$markdown.AppendLine("| **Medium** (60-79%) | $medium | $([Math]::Round($medium / $files.Count * 100, 1))% |")
[void]$markdown.AppendLine("| **Low** (1-59%) | $low | $([Math]::Round($low / $files.Count * 100, 1))% |")
[void]$markdown.AppendLine("| **No Coverage** (0%) | $zero | $([Math]::Round($zero / $files.Count * 100, 1))% |")
[void]$markdown.AppendLine("")

[void]$markdown.AppendLine("## File Coverage Tree")
[void]$markdown.AppendLine("")
if (-not [string]::IsNullOrEmpty($basePath)) {
    [void]$markdown.AppendLine("> **Base Path:** ``$basePath``  ")
}
[void]$markdown.AppendLine("> **Legend:** `[V]` Perfect (100%) · `[+]` Excellent (≥90%) · `[ ]` Good (≥80%) · `[~]` Fair (≥60%) · `[!]` Poor (<60%) · `[.]` No Coverage (0%)")
[void]$markdown.AppendLine("")
[void]$markdown.AppendLine("*Click on folders to expand/collapse sections*")
[void]$markdown.AppendLine("")
[void]$markdown.AppendLine("---")
[void]$markdown.AppendLine("")

$tree = Build-FolderTree -Files $files -BasePath $basePath
Render-FolderTree -Tree $tree -Markdown $markdown -MinCoverage $MinimumCoverage

$uncovered = $files | Where-Object { $_.Coverage -lt $MinimumCoverage } | Sort-Object Coverage | Select-Object -First 20

if ($uncovered.Count -gt 0) {
    [void]$markdown.AppendLine("---")
    [void]$markdown.AppendLine("")
    [void]$markdown.AppendLine("## Files Needing Attention")
    [void]$markdown.AppendLine("")
    [void]$markdown.AppendLine("> **$($uncovered.Count) file(s)** below $MinimumCoverage% coverage threshold")
    [void]$markdown.AppendLine("")
    [void]$markdown.AppendLine("| Priority | File | Coverage | Statements | Uncovered |")
    [void]$markdown.AppendLine("|----------|------|----------|------------|-----------|")
    
    foreach ($file in $uncovered) {
        $icon = Get-CoverageIcon -Coverage $file.Coverage
        
        # Use SVG badge icons for priority (no emojis to avoid encoding issues)
        if ($file.Coverage -eq 0) {
            $priority = "![Critical](https://img.shields.io/badge/CRITICAL-red?style=flat-square)"
        } elseif ($file.Coverage -lt 25) {
            $priority = "![High](https://img.shields.io/badge/HIGH-orange?style=flat-square)"
        } elseif ($file.Coverage -lt 50) {
            $priority = "![Medium](https://img.shields.io/badge/MEDIUM-yellow?style=flat-square)"
        } else {
            $priority = "![Low](https://img.shields.io/badge/LOW-blue?style=flat-square)"
        }
        
        $badge = Get-CoverageLabel -Coverage $file.Coverage
        $uncoveredCount = $file.TotalStatements - $file.CoveredStatements
        $relativePath = Get-RelativePath -Path $file.Path -BasePath $basePath
        
        [void]$markdown.AppendLine("| $priority | $icon ``$relativePath`` | $badge | $(Format-Number $file.TotalStatements) | $(Format-Number $uncoveredCount) |")
    }
    
    [void]$markdown.AppendLine("")
}

[void]$markdown.AppendLine("---")
[void]$markdown.AppendLine("")
[void]$markdown.AppendLine("*Detailed report generated by Shotora Coverage Tool | Powered by DotCover $dotCoverVersion*")

$markdown.ToString() | Out-File -FilePath $OutputFilePath -Encoding UTF8

Write-Host ""
Write-Host "[SUCCESS] Detailed report generated!" -ForegroundColor Green
Write-Host "[OUTPUT] Report saved to: $OutputFilePath" -ForegroundColor Cyan
Write-Host ""

Write-Host "========== SUMMARY ==========" -ForegroundColor Cyan
Write-Host "Overall Coverage: $overallCoverage%" -ForegroundColor White
Write-Host "Total Files: $($files.Count)" -ForegroundColor White
Write-Host "Perfect Coverage: $perfect files" -ForegroundColor Green
Write-Host "High Coverage: $high files" -ForegroundColor Green
Write-Host "Medium Coverage: $medium files" -ForegroundColor Yellow
Write-Host "Low Coverage: $low files" -ForegroundColor Red
Write-Host "No Coverage: $zero files" -ForegroundColor Red
if ($uncovered.Count -gt 0) {
    Write-Host "Files Below Threshold: $($uncovered.Count)" -ForegroundColor Magenta
}
Write-Host "============================" -ForegroundColor Cyan
Write-Host ""

#endregion
