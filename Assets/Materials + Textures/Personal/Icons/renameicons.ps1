# Requires Windows with .NET and System.Drawing.Common
Add-Type -AssemblyName System.Drawing

# Set source and destination folders
$sourceFolder = "C:\Users\callu\OneDrive\Pictures\Textures\Game Icons"
$destinationFolder = "C:\Users\callu\OneDrive\Pictures\Textures\Game Icons\Resized"
$targetSize = 44  # Target width and height in pixels

# Create destination folder if it doesn't exist
if (!(Test-Path $destinationFolder)) {
    New-Item -ItemType Directory -Path $destinationFolder
}

# Process each image file
Get-ChildItem -Path $sourceFolder -Filter *.png | Where-Object { $_.Name.ToLower() -like "*icon*" } | ForEach-Object {
    $sourcePath = $_.FullName
    $destPath = Join-Path $destinationFolder $_.Name

    $original = [System.Drawing.Image]::FromFile($sourcePath)
    $resized = New-Object System.Drawing.Bitmap $targetSize, $targetSize
    $graphics = [System.Drawing.Graphics]::FromImage($resized)

    $graphics.CompositingQuality = [System.Drawing.Drawing2D.CompositingQuality]::HighQuality
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::HighQuality
    $graphics.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
    $graphics.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
    $graphics.DrawImage($original, 0, 0, $targetSize, $targetSize)

    $resized.Save($destPath, [System.Drawing.Imaging.ImageFormat]::Png)

    $graphics.Dispose()
    $original.Dispose()
    $resized.Dispose()

    Write-Host "Resized: $($_.Name)"
}