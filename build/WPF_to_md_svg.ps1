# =========================
# WPF Geometry to SVG to Markdown <img>
# Converts WPF path geometries to inline SVG data URIs
# =========================

Add-Type -AssemblyName PresentationCore

function To-SvgNumber([double]$v) {
    $v.ToString("0.###", [Globalization.CultureInfo]::InvariantCulture)
}

function PathGeometry-ToSvgPathData([System.Windows.Media.PathGeometry]$pg) {
    $sb = New-Object System.Text.StringBuilder

    foreach ($fig in $pg.Figures) {
        if (-not $fig.IsFilled) { continue }

        [void]$sb.Append("M")
        [void]$sb.Append((To-SvgNumber $fig.StartPoint.X))
        [void]$sb.Append(",")
        [void]$sb.Append((To-SvgNumber $fig.StartPoint.Y))

        foreach ($seg in $fig.Segments) {
            switch ($seg) {
                { $_ -is [System.Windows.Media.LineSegment] } {
                    [void]$sb.Append("L")
                    [void]$sb.Append((To-SvgNumber $seg.Point.X))
                    [void]$sb.Append(",")
                    [void]$sb.Append((To-SvgNumber $seg.Point.Y))
                }
                { $_ -is [System.Windows.Media.PolyLineSegment] } {
                    foreach ($p in $seg.Points) {
                        [void]$sb.Append("L")
                        [void]$sb.Append((To-SvgNumber $p.X))
                        [void]$sb.Append(",")
                        [void]$sb.Append((To-SvgNumber $p.Y))
                    }
                }
                { $_ -is [System.Windows.Media.BezierSegment] } {
                    [void]$sb.Append("C")
                    [void]$sb.Append((To-SvgNumber $seg.Point1.X))
                    [void]$sb.Append(",")
                    [void]$sb.Append((To-SvgNumber $seg.Point1.Y))
                    [void]$sb.Append(",")
                    [void]$sb.Append((To-SvgNumber $seg.Point2.X))
                    [void]$sb.Append(",")
                    [void]$sb.Append((To-SvgNumber $seg.Point2.Y))
                    [void]$sb.Append(",")
                    [void]$sb.Append((To-SvgNumber $seg.Point3.X))
                    [void]$sb.Append(",")
                    [void]$sb.Append((To-SvgNumber $seg.Point3.Y))
                }
                { $_ -is [System.Windows.Media.PolyBezierSegment] } {
                    for ($i = 0; $i -lt $seg.Points.Count; $i += 3) {
                        $p1 = $seg.Points[$i]
                        $p2 = $seg.Points[$i + 1]
                        $p3 = $seg.Points[$i + 2]
                        [void]$sb.Append("C")
                        [void]$sb.Append((To-SvgNumber $p1.X))
                        [void]$sb.Append(",")
                        [void]$sb.Append((To-SvgNumber $p1.Y))
                        [void]$sb.Append(",")
                        [void]$sb.Append((To-SvgNumber $p2.X))
                        [void]$sb.Append(",")
                        [void]$sb.Append((To-SvgNumber $p2.Y))
                        [void]$sb.Append(",")
                        [void]$sb.Append((To-SvgNumber $p3.X))
                        [void]$sb.Append(",")
                        [void]$sb.Append((To-SvgNumber $p3.Y))
                    }
                }
                { $_ -is [System.Windows.Media.QuadraticBezierSegment] } {
                    [void]$sb.Append("Q")
                    [void]$sb.Append((To-SvgNumber $seg.Point1.X))
                    [void]$sb.Append(",")
                    [void]$sb.Append((To-SvgNumber $seg.Point1.Y))
                    [void]$sb.Append(",")
                    [void]$sb.Append((To-SvgNumber $seg.Point2.X))
                    [void]$sb.Append(",")
                    [void]$sb.Append((To-SvgNumber $seg.Point2.Y))
                }
                { $_ -is [System.Windows.Media.PolyQuadraticBezierSegment] } {
                    for ($i = 0; $i -lt $seg.Points.Count; $i += 2) {
                        $c = $seg.Points[$i]
                        $e = $seg.Points[$i + 1]
                        [void]$sb.Append("Q")
                        [void]$sb.Append((To-SvgNumber $c.X))
                        [void]$sb.Append(",")
                        [void]$sb.Append((To-SvgNumber $c.Y))
                        [void]$sb.Append(",")
                        [void]$sb.Append((To-SvgNumber $e.X))
                        [void]$sb.Append(",")
                        [void]$sb.Append((To-SvgNumber $e.Y))
                    }
                }
                { $_ -is [System.Windows.Media.ArcSegment] } {
                    $rx = [Math]::Abs($seg.Size.Width)
                    $ry = [Math]::Abs($seg.Size.Height)
                    $largeArc = if ($seg.IsLargeArc) { 1 } else { 0 }
                    $sweep = if ($seg.SweepDirection -eq [System.Windows.Media.SweepDirection]::Clockwise) { 1 } else { 0 }

                    [void]$sb.Append("A")
                    [void]$sb.Append((To-SvgNumber $rx))
                    [void]$sb.Append(",")
                    [void]$sb.Append((To-SvgNumber $ry))
                    [void]$sb.Append(",")
                    [void]$sb.Append((To-SvgNumber $seg.RotationAngle))
                    [void]$sb.Append(",")
                    [void]$sb.Append($largeArc)
                    [void]$sb.Append(",")
                    [void]$sb.Append($sweep)
                    [void]$sb.Append(",")
                    [void]$sb.Append((To-SvgNumber $seg.Point.X))
                    [void]$sb.Append(",")
                    [void]$sb.Append((To-SvgNumber $seg.Point.Y))
                }
                default {
                    throw "Unsupported segment type: $($seg.GetType().FullName)"
                }
            }
        }

        if ($fig.IsClosed) { [void]$sb.Append("Z") }
    }

    $sb.ToString().Trim()
}

function Make-OutlinedPathGeometry([System.Windows.Media.Geometry]$geo, [double]$tolerance = 0.25) {
    # Stabilizes fill/holes by converting to an outline of the filled region
    # Higher tolerance = more simplification (fewer points)
    $outlined = $geo.GetOutlinedPathGeometry()
    if ($tolerance -gt 0.25) {
        # Use flattening to reduce complexity
        $outlined = $outlined.GetFlattenedPathGeometry($tolerance, [System.Windows.Media.ToleranceType]::Absolute)
    }
    return $outlined
}

# -------- INPUT (your block) --------
$geometries = @"
<Geometry x:Key="IconPen">m499-287 335-335-52-52-335 335 52 52Zm-261 87q-100-5-149-42T40-349q0-65 53.5-105.5T242-503q39-3 58.5-12.5T320-542q0-26-29.5-39T193-600l7-80q103 8 151.5 41.5T400-542q0 53-38.5 83T248-423q-64 5-96 23.5T120-349q0 35 28 50.5t94 18.5l-4 80Zm280 7L353-358l382-382q20-20 47.5-20t47.5 20l70 70q20 20 20 47.5T900-575L518-193Zm-159 33q-17 4-30-9t-9-30l33-159 165 165-159 33Z</Geometry>
<Geometry x:Key="IconLine">M212-212q-11-11-11-28t11-28l480-480q11-12 27.5-12t28.5 12q11 11 11 28t-11 28L268-212q-11 11-28 11t-28-11Z</Geometry>
<Geometry x:Key="IconArrow">m136-240-56-56 212-212q35-35 85-35t85 35l46 46q12 12 28.5 12t28.5-12l178-178H640v-80h240v240h-80v-103L621-405q-35 35-85 35t-85-35l-47-47q-11-11-28-11t-28 11L136-240Z</Geometry>
<Geometry x:Key="IconRect">M80-160v-640h800v640H80Zm80-80h640v-480H160v480Z</Geometry>
<Geometry x:Key="IconEllipse">M480-80q-83 0-156-31.5T197-197q-54-54-85.5-127T80-480q0-83 31.5-156T197-763q54-54 127-85.5T480-880q83 0 156 31.5T763-763q54 54 85.5 127T880-480q0 83-31.5 156T763-197q-54 54-127 85.5T480-80Zm0-80q134 0 227-93t93-227q0-134-93-227t-227-93q-134 0-227 93t-93 227q0 134 93 227t227 93Zm0-320Z</Geometry>
<Geometry x:Key="IconText">M280-160v-520H80v-120h520v120H400v520H280Zm360 0v-320H520v-120h360v120H760v320H640Z</Geometry>
<Geometry x:Key="IconHighlight">M544-400 440-504 240-304l104 104 200-200Zm-47-161 104 104 199-199-104-104-199 199Zm-84-28 216 216-229 229q-24 24-56 24t-56-24l-2-2-26 26H60l126-126-2-2q-24-24-24-56t24-56l229-229Zm0 0 227-227q24-24 56-24t56 24l104 104q24 24 24 56t-24 56L629-373 413-589Z</Geometry>
<Geometry x:Key="IconBlur">M120-380q-8 0-14-6t-6-14q0-8 6-14t14-6q8 0 14 6t6 14q0 8-6 14t-14 6Zm0-160q-8 0-14-6t-6-14q0-8 6-14t14-6q8 0 14 6t6 14q0 8-6 14t-14 6Zm120 340q-17 0-28.5-11.5T200-240q0-17 11.5-28.5T240-280q17 0 28.5 11.5T280-240q0 17-11.5 28.5T240-200Zm0-160q-17 0-28.5-11.5T200-400q0-17 11.5-28.5T240-440q17 0 28.5 11.5T280-400q0 17-11.5 28.5T240-360Zm0-160q-17 0-28.5-11.5T200-560q0-17 11.5-28.5T240-600q17 0 28.5 11.5T280-560q0 17-11.5 28.5T240-520Zm0-160q-17 0-28.5-11.5T200-720q0-17 11.5-28.5T240-760q17 0 28.5 11.5T280-720q0 17-11.5 28.5T240-680Zm160 340q-25 0-42.5-17.5T340-400q0-25 17.5-42.5T400-460q25 0 42.5 17.5T460-400q0 25-17.5 42.5T400-340Zm0-160q-25 0-42.5-17.5T340-560q0-25 17.5-42.5T400-620q25 0 42.5 17.5T460-560q0 25-17.5 42.5T400-500Zm0 300q-17 0-28.5-11.5T360-240q0-17 11.5-28.5T400-280q17 0 28.5 11.5T440-240q0 17-11.5 28.5T400-200Zm0-480q-17 0-28.5-11.5T360-720q0-17 11.5-28.5T400-760q17 0 28.5 11.5T440-720q0 17-11.5 28.5T400-680Zm0 580q-8 0-14-6t-6-14q0-8 6-14t14-6q8 0 14 6t6 14q0 8-6 14t-14 6Zm0-720q-8 0-14-6t-6-14q0-8 6-14t14-6q8 0 14 6t6 14q0 8-6 14t-14 6Zm160 480q-25 0-42.5-17.5T500-400q0-25 17.5-42.5T560-460q25 0 42.5 17.5T620-400q0 25-17.5 42.5T560-340Zm0-160q-25 0-42.5-17.5T500-560q0-25 17.5-42.5T560-620q25 0 42.5 17.5T620-560q0 25-17.5 42.5T560-500Zm0 300q-17 0-28.5-11.5T520-240q0-17 11.5-28.5T560-280q17 0 28.5 11.5T600-240q0 17-11.5 28.5T560-200Zm0-480q-17 0-28.5-11.5T520-720q0-17 11.5-28.5T560-760q17 0 28.5 11.5T600-720q0 17-11.5 28.5T560-680Zm0 580q-8 0-14-6t-6-14q0-8 6-14t14-6q8 0 14 6t6 14q0 8-6 14t-14 6Zm0-720q-8 0-14-6t-6-14q0-8 6-14t14-6q8 0 14 6t6 14q0 8-6 14t-14 6Zm160 620q-17 0-28.5-11.5T680-240q0-17 11.5-28.5T720-280q17 0 28.5 11.5T760-240q0 17-11.5 28.5T720-200Zm0-160q-17 0-28.5-11.5T680-400q0-17 11.5-28.5T720-440q17 0 28.5 11.5T760-400q0 17-11.5 28.5T720-360Zm0-160q-17 0-28.5-11.5T680-560q0-17 11.5-28.5T720-600q17 0 28.5 11.5T760-560q0 17-11.5 28.5T720-520Zm0-160q-17 0-28.5-11.5T680-720q0-17 11.5-28.5T720-760q17 0 28.5 11.5T760-720q0 17-11.5 28.5T720-680Zm120 300q-8 0-14-6t-6-14q0-8 6-14t14-6q8 0 14 6t6 14q0 8-6 14t-14 6Zm0-160q-8 0-14-6t-6-14q0-8 6-14t14-6q8 0 14 6t6 14q0 8-6 14t-14 6Z</Geometry>
<Geometry x:Key="IconPixelate">M200-120q-33 0-56.5-23.5T120-200v-560q0-33 23.5-56.5T200-840h200v720H200Zm280 0q-17 0-28.5-11.5T440-160q0-17 11.5-28.5T480-200q17 0 28.5 11.5T520-160q0 17-11.5 28.5T480-120Zm0-160q-17 0-28.5-11.5T440-320q0-17 11.5-28.5T480-360q17 0 28.5 11.5T520-320q0 17-11.5 28.5T480-280Zm0-160q-17 0-28.5-11.5T440-480q0-17 11.5-28.5T480-520q17 0 28.5 11.5T520-480q0 17-11.5 28.5T480-440Zm0-160q-17 0-28.5-11.5T440-640q0-17 11.5-28.5T480-680q17 0 28.5 11.5T520-640q0 17-11.5 28.5T480-600Zm0-160q-17 0-28.5-11.5T440-800q0-17 11.5-28.5T480-840q17 0 28.5 11.5T520-800q0 17-11.5 28.5T480-760Zm80 560q-17 0-28.5-11.5T520-240q0-17 11.5-28.5T560-280q17 0 28.5 11.5T600-240q0 17-11.5 28.5T560-200Zm0-160q-17 0-28.5-11.5T520-400q0-17 11.5-28.5T560-440q17 0 28.5 11.5T600-400q0 17-11.5 28.5T560-360Zm0-160q-17 0-28.5-11.5T520-560q0-17 11.5-28.5T560-600q17 0 28.5 11.5T600-560q0 17-11.5 28.5T560-520Zm0-160q-17 0-28.5-11.5T520-720q0-17 11.5-28.5T560-760q17 0 28.5 11.5T600-720q0 17-11.5 28.5T560-680Zm80 560q-17 0-28.5-11.5T600-160q0-17 11.5-28.5T640-200q17 0 28.5 11.5T680-160q0 17-11.5 28.5T640-120Zm0-160q-17 0-28.5-11.5T600-320q0-17 11.5-28.5T640-360q17 0 28.5 11.5T680-320q0 17-11.5 28.5T640-280Zm0-160q-17 0-28.5-11.5T600-480q0-17 11.5-28.5T640-520q17 0 28.5 11.5T680-480q0 17-11.5 28.5T640-440Zm0-160q-17 0-28.5-11.5T600-640q0-17 11.5-28.5T640-680q17 0 28.5 11.5T680-640q0 17-11.5 28.5T640-600Zm0-160q-17 0-28.5-11.5T600-800q0-17 11.5-28.5T640-840q17 0 28.5 11.5T680-800q0 17-11.5 28.5T640-760Zm80 560q-17 0-28.5-11.5T680-240q0-17 11.5-28.5T720-280q17 0 28.5 11.5T760-240q0 17-11.5 28.5T720-200Zm0-160q-17 0-28.5-11.5T680-400q0-17 11.5-28.5T720-440q17 0 28.5 11.5T760-400q0 17-11.5 28.5T720-360Zm0-160q-17 0-28.5-11.5T680-560q0-17 11.5-28.5T720-600q17 0 28.5 11.5T760-560q0 17-11.5 28.5T720-520Zm0-160q-17 0-28.5-11.5T680-720q0-17 11.5-28.5T720-760q17 0 28.5 11.5T760-720q0 17-11.5 28.5T720-680Zm120 300q-8 0-14-6t-6-14q0-8 6-14t14-6q8 0 14 6t6 14q0 8-6 14t-14 6Zm0-160q-8 0-14-6t-6-14q0-8 6-14t14-6q8 0 14 6t6 14q0 8-6 14t-14 6Z</Geometry>
"@

$outFile = "\result.txt"
Remove-Item $outFile -ErrorAction Ignore

# Parse geometries into a lookup
$regex = '<Geometry\s+x:Key="([^"]+)">(.*?)</Geometry>'
$matches = [regex]::Matches($geometries, $regex, 'Singleline')

$geoMap = @{}
foreach ($m in $matches) {
    $k = $m.Groups[1].Value
    $d = $m.Groups[2].Value.Trim()
    $geoMap[$k] = [System.Windows.Media.Geometry]::Parse($d)
}

# Visual output settings
$iconFill   = "#AD64f5"
$imgSizePx  = 15
$padding    = 0

foreach ($key in $geoMap.Keys) {

    $geo = $geoMap[$key]

    # Try with different simplification levels until it fits under 1500 chars
    $tolerances = @(0.25, 1, 2, 3, 4, 5)
    $success = $false
    $lastValidResult = $null

    foreach ($tolerance in $tolerances) {
        try {
            # Outline the filled region for stable rendering of compound shapes
            $outlined = Make-OutlinedPathGeometry $geo $tolerance

            # Skip if geometry becomes empty
            if ($outlined.Figures.Count -eq 0) {
                Write-Host "WARNING: $key became empty at tolerance $tolerance" -ForegroundColor Yellow
                break
            }

            # ViewBox from bounds
            $b = $outlined.Bounds
            $minX = $b.X - $padding
            $minY = $b.Y - $padding
            $w    = $b.Width  + ($padding * 2)
            $h    = $b.Height + ($padding * 2)

            $d = PathGeometry-ToSvgPathData $outlined

            # Skip if path data is empty
            if ([string]::IsNullOrWhiteSpace($d)) {
                Write-Host "WARNING: $key path became empty at tolerance $tolerance" -ForegroundColor Yellow
                break
            }

            # Minify SVG (single line, no extra whitespace, use single quotes to reduce encoding)
            $svg = "<svg xmlns='http://www.w3.org/2000/svg' viewBox='$(To-SvgNumber $minX) $(To-SvgNumber $minY) $(To-SvgNumber $w) $(To-SvgNumber $h)'><path fill='$iconFill' fill-rule='evenodd' d='$d'/></svg>"

            $b64 = [Convert]::ToBase64String([Text.Encoding]::UTF8.GetBytes($svg))
            $md = " ![](https://img.shields.io/badge/-white?&logo=data:image/svg+xml;base64,$b64)"

            # Store as last valid result
            $lastValidResult = @{
                md = $md
                length = $b64.Length
                tolerance = $tolerance
            }

            # Check if it fits (1500 is safe limit for shields.io URLs)
            if ($b64.Length -le 1500) {
                if ($tolerance -gt 0.25) {
                    Write-Host "OK: $key base64 length is $($b64.Length) (simplified with tolerance $tolerance)" -ForegroundColor Cyan
                } else {
                    Write-Host "OK: $key base64 length is $($b64.Length)" -ForegroundColor Green
                }

                Add-Content $outFile $md
                Add-Content $outFile ""
                $success = $true
                break
            }
        }
        catch {
            Write-Host "WARNING: $key failed at tolerance $tolerance : $_" -ForegroundColor Yellow
            break
        }
    }

    # If we couldn't fit under 1500, use the best result we got
    if (-not $success -and $lastValidResult) {
        Write-Host "WARNING: $key using best result (length: $($lastValidResult.length), tolerance: $($lastValidResult.tolerance))" -ForegroundColor Magenta
        Add-Content $outFile $lastValidResult.md
        Add-Content $outFile ""
    }
    elseif (-not $success) {
        Write-Host "ERROR: $key could not be processed" -ForegroundColor Red
    }
}

Write-Host "DONE - result.txt (SVG icons exported)"