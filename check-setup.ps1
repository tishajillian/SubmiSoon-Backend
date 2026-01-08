# SubmiSoon Setup Verification Script
# This script checks if all prerequisites are installed and configured correctly

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "   SubmiSoon Setup Verification" -ForegroundColor Cyan
Write-Host "========================================`n" -ForegroundColor Cyan

$allChecksPass = $true

# Check 1: .NET 8 SDK
Write-Host "Checking .NET 8 SDK..." -ForegroundColor Yellow
try {
    $dotnetVersion = dotnet --version 2>$null
    if ($dotnetVersion) {
        $majorVersion = [int]($dotnetVersion.Split('.')[0])
        if ($majorVersion -ge 8) {
            Write-Host "  [PASS] .NET SDK $dotnetVersion is installed" -ForegroundColor Green
        } else {
            Write-Host "  [FAIL] .NET SDK $dotnetVersion found, but version 8.0 or higher is required" -ForegroundColor Red
            Write-Host "         Download from: https://dotnet.microsoft.com/download/dotnet/8.0" -ForegroundColor Gray
            $allChecksPass = $false
        }
    } else {
        Write-Host "  [FAIL] .NET SDK not found" -ForegroundColor Red
        Write-Host "         Download from: https://dotnet.microsoft.com/download/dotnet/8.0" -ForegroundColor Gray
        $allChecksPass = $false
    }
} catch {
    Write-Host "  [FAIL] .NET SDK not found" -ForegroundColor Red
    Write-Host "         Download from: https://dotnet.microsoft.com/download/dotnet/8.0" -ForegroundColor Gray
    $allChecksPass = $false
}

Write-Host ""

# Check 2: SQL Server
Write-Host "Checking SQL Server..." -ForegroundColor Yellow

$sqlServerFound = $false
$sqlServices = @(
    "MSSQLSERVER",           # Default instance
    "MSSQL`$SQLEXPRESS",     # SQL Express
    "SQLWriter"              # SQL Server VSS Writer (indicates SQL is installed)
)

foreach ($serviceName in $sqlServices) {
    $service = Get-Service -Name $serviceName -ErrorAction SilentlyContinue
    if ($service) {
        $sqlServerFound = $true
        $status = $service.Status
        $displayName = $service.DisplayName
        
        if ($status -eq "Running") {
            Write-Host "  [PASS] $displayName is installed and running" -ForegroundColor Green
        } else {
            Write-Host "  [WARN] $displayName is installed but not running (Status: $status)" -ForegroundColor Yellow
            Write-Host "         You may need to start the service manually" -ForegroundColor Gray
        }
        break
    }
}

if (-not $sqlServerFound) {
    Write-Host "  [FAIL] SQL Server not found" -ForegroundColor Red
    Write-Host "         Download SQL Server Express (free) from:" -ForegroundColor Gray
    Write-Host "         https://www.microsoft.com/en-us/sql-server/sql-server-downloads" -ForegroundColor Gray
    $allChecksPass = $false
}

Write-Host ""

# Check 3: Project file exists
Write-Host "Checking project structure..." -ForegroundColor Yellow
$projectFile = "SubmiSoonProject\SubmiSoonProject.csproj"
if (Test-Path $projectFile) {
    Write-Host "  [PASS] Project file found: $projectFile" -ForegroundColor Green
} else {
    Write-Host "  [FAIL] Project file not found: $projectFile" -ForegroundColor Red
    Write-Host "         Make sure you're running this script from the repository root" -ForegroundColor Gray
    $allChecksPass = $false
}

Write-Host ""

# Check 4: Connection string validation
Write-Host "Checking configuration..." -ForegroundColor Yellow
$appsettingsFile = "SubmiSoonProject\appsettings.json"
if (Test-Path $appsettingsFile) {
    Write-Host "  [PASS] Configuration file found: $appsettingsFile" -ForegroundColor Green
    
    # Try to parse and check connection string
    try {
        $config = Get-Content $appsettingsFile -Raw | ConvertFrom-Json
        $connString = $config.ConnectionStrings.DefaultConnection
        if ($connString) {
            Write-Host "  [INFO] Connection string configured" -ForegroundColor Cyan
            Write-Host "         Database: SubmiSoonDb" -ForegroundColor Gray
            Write-Host "         Server: localhost" -ForegroundColor Gray
        } else {
            Write-Host "  [WARN] Connection string not found in configuration" -ForegroundColor Yellow
        }
    } catch {
        Write-Host "  [WARN] Could not parse configuration file" -ForegroundColor Yellow
    }
} else {
    Write-Host "  [FAIL] Configuration file not found: $appsettingsFile" -ForegroundColor Red
    $allChecksPass = $false
}

Write-Host ""

# Summary
Write-Host "========================================" -ForegroundColor Cyan
if ($allChecksPass) {
    Write-Host "   All checks passed!" -ForegroundColor Green
    Write-Host "========================================`n" -ForegroundColor Cyan
    Write-Host "You're ready to run SubmiSoon!" -ForegroundColor Green
    Write-Host "`nNext steps:" -ForegroundColor White
    Write-Host "  1. Run: " -NoNewline -ForegroundColor White
    Write-Host "dotnet run --project SubmiSoonProject" -ForegroundColor Cyan
    Write-Host "  2. Open: " -NoNewline -ForegroundColor White
    Write-Host "https://localhost:7123/swagger" -ForegroundColor Cyan
    Write-Host "  3. Login with: " -NoNewline -ForegroundColor White
    Write-Host "tisha.jillian@sunib.ac.id / password2k25" -ForegroundColor Cyan
} else {
    Write-Host "   Some checks failed" -ForegroundColor Red
    Write-Host "========================================`n" -ForegroundColor Cyan
    Write-Host "Please install the missing prerequisites and run this script again." -ForegroundColor Yellow
    Write-Host "See README.md for detailed installation instructions." -ForegroundColor Gray
}

Write-Host ""

# Optional: EF Core tools check
Write-Host "Optional Tools Check:" -ForegroundColor Yellow
try {
    $efVersion = dotnet ef --version 2>$null
    if ($efVersion) {
        Write-Host "  [INFO] Entity Framework Core tools installed: $efVersion" -ForegroundColor Cyan
    } else {
        Write-Host "  [INFO] Entity Framework Core tools not installed (optional)" -ForegroundColor Gray
        Write-Host "         Install with: dotnet tool install --global dotnet-ef" -ForegroundColor Gray
        Write-Host "         (Only needed for manual database migrations)" -ForegroundColor Gray
    }
} catch {
    Write-Host "  [INFO] Entity Framework Core tools not installed (optional)" -ForegroundColor Gray
    Write-Host "         Install with: dotnet tool install --global dotnet-ef" -ForegroundColor Gray
    Write-Host "         (Only needed for manual database migrations)" -ForegroundColor Gray
}

Write-Host ""
