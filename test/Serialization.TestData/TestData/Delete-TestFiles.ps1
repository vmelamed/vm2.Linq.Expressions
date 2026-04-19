# PowerShell script to delete all files except those in sub-folders named "LoadTestData" anywhere in the directory tree

param(
    [Parameter(Mandatory = $false)]
    [string]$RootPath = "./TestData",

    [Parameter(Mandatory = $false)]
    [string]$ExcludeFolderName = "LoadTestData",

    [Parameter(Mandatory = $false)]
    [switch]$WhatIf,

    [Parameter(Mandatory = $false)]
    [switch]$Recurse
)

# Function to check if a file is within any excluded folder in the directory tree
function Test-FileInExcludedFolder {
    param(
        [string]$FilePath,
        [string]$ExcludedFolderName
    )

    $pathParts = $FilePath.Split([System.IO.Path]::DirectorySeparatorChar, [System.StringSplitOptions]::RemoveEmptyEntries)

    # Check if any part of the path matches the excluded folder name
    foreach ($part in $pathParts) {
        if ($part -eq $ExcludedFolderName) {
            return $true
        }
    }

    return $false
}

# Validate root path
if (-not (Test-Path $RootPath)) {
    Write-Error "Root path '$RootPath' does not exist."
    exit 1
}

# Get the absolute path
$RootPath = Resolve-Path $RootPath

Write-Host "Starting file deletion process..." -ForegroundColor Green
Write-Host "Root Path: $RootPath" -ForegroundColor Cyan
Write-Host "Excluded Folder Name: $ExcludeFolderName" -ForegroundColor Cyan
Write-Host "What-If Mode: $WhatIf" -ForegroundColor Cyan
Write-Host ""

# Get all files recursively
$allFiles = Get-ChildItem -Path $RootPath -File -Recurse:$(!$Recurse.IsPresent -or $Recurse)

$filesToDelete = @()
$filesSkipped = @()

foreach ($file in $allFiles) {
    $relativePath = $file.FullName.Substring($RootPath.Length).TrimStart([System.IO.Path]::DirectorySeparatorChar)

    if (Test-FileInExcludedFolder -FilePath $relativePath -ExcludedFolderName $ExcludeFolderName) {
        $filesSkipped += $file
        Write-Host "SKIPPED: $($file.FullName)" -ForegroundColor Yellow
    }
    else {
        $filesToDelete += $file
        if ($WhatIf) {
            Write-Host "WOULD DELETE: $($file.FullName)" -ForegroundColor Red
        }
        else {
            Write-Host "DELETING: $($file.FullName)" -ForegroundColor Red
        }
    }
}

Write-Host ""
Write-Host "Summary:" -ForegroundColor Green
Write-Host "Files to delete: $($filesToDelete.Count)" -ForegroundColor Red
Write-Host "Files skipped (in $ExcludeFolderName folders): $($filesSkipped.Count)" -ForegroundColor Yellow

if ($filesToDelete.Count -eq 0) {
    Write-Host "No files to delete." -ForegroundColor Green
    exit 0
}

if ($WhatIf) {
    Write-Host ""
    Write-Host "This was a dry run. Use without -WhatIf to actually delete the files." -ForegroundColor Magenta
    exit 0
}

# Confirm deletion if not in WhatIf mode
Write-Host ""
$confirmation = Read-Host "Are you sure you want to delete $($filesToDelete.Count) files? (y/N)"

if ($confirmation -eq 'y' -or $confirmation -eq 'Y' -or $confirmation -eq 'yes' -or $confirmation -eq 'YES') {
    $deletedCount = 0
    $errorCount = 0

    foreach ($file in $filesToDelete) {
        try {
            Remove-Item -Path $file.FullName -Force
            $deletedCount++
        }
        catch {
            Write-Error "Failed to delete $($file.FullName): $($_.Exception.Message)"
            $errorCount++
        }
    }

    Write-Host ""
    Write-Host "Deletion completed!" -ForegroundColor Green
    Write-Host "Files deleted: $deletedCount" -ForegroundColor Green
    Write-Host "Errors: $errorCount" -ForegroundColor $(if ($errorCount -gt 0) { "Red" } else { "Green" })
}
else {
    Write-Host "Operation cancelled by user." -ForegroundColor Yellow
}
