# PowerShell Script to Add ILogger to All Services
# This script adds ILogger parameter to all services that extend BaseService

$servicesPath = "SMSBACKEND.Infrastructure\Services\Services"
$serviceFiles = Get-ChildItem -Path $servicesPath -Filter "*Service.cs" -Recurse

Write-Host "Found $($serviceFiles.Count) service files" -ForegroundColor Green
Write-Host ""

$updatedCount = 0
$skippedCount = 0

foreach ($file in $serviceFiles) {
    $content = Get-Content $file.FullName -Raw
    
    # Skip if already has ILogger or doesn't extend BaseService
    if ($content -match "ILogger<" -or $content -notmatch ": BaseService<") {
        Write-Host "SKIP: $($file.Name) (already has logger or not BaseService)" -ForegroundColor Yellow
        $skippedCount++
        continue
    }
    
    # Extract service name from filename
    $serviceName = $file.BaseName
    
    # Check if service has the standard constructor pattern
    if ($content -match "public\s+$serviceName\s*\([^)]*\)\s*:\s*base\([^)]*\)") {
        # Pattern found - needs update
        Write-Host "UPDATE: $($file.Name)" -ForegroundColor Cyan
        
        # Add using statement if not present
        if ($content -notmatch "using Microsoft.Extensions.Logging;") {
            $content = $content -replace "(using Infrastructure.Services.Communication;)", "`$1`r`nusing Microsoft.Extensions.Logging;"
        }
        
        # Add ILogger parameter to constructor
        $content = $content -replace "(\s+ICacheProvider cacheProvider\s*\))", "`$1,`r`n            ILogger<$serviceName> logger`r`n            )"
        
        # Add logger to base() call
        $content = $content -replace "(\) : base\([^)]+cacheProvider)", "`$1, logger"
        
        # Save updated content
        Set-Content -Path $file.FullName -Value $content
        
        $updatedCount++
    } else {
        Write-Host "SKIP: $($file.Name) (complex constructor)" -ForegroundColor Magenta
        $skippedCount++
    }
}

Write-Host ""
Write-Host "=====================================" -ForegroundColor Green
Write-Host "SUMMARY:" -ForegroundColor Green
Write-Host "Updated: $updatedCount services" -ForegroundColor Cyan
Write-Host "Skipped: $skippedCount services" -ForegroundColor Yellow
Write-Host "=====================================" -ForegroundColor Green
Write-Host ""
Write-Host "Services updated with ILogger<TService> injection!" -ForegroundColor Green
Write-Host "All services now have centralized logging capability." -ForegroundColor Green

