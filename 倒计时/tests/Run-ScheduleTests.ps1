$ErrorActionPreference = 'Stop'
$scheduleRoot = Split-Path -Parent $PSScriptRoot
$scheduleAssets = Get-Content -Raw -Encoding UTF8 -LiteralPath (Join-Path $scheduleRoot 'obj/project.assets.json') | ConvertFrom-Json
$schedulePackages = $scheduleAssets.packageFolders.PSObject.Properties.Name | Where-Object {
    Test-Path -LiteralPath (Join-Path $_ 'sqlite.net-pcl/3.1.1/lib/net40/SQLite.Net.Platform.Generic.dll')
} | Select-Object -First 1
if (-not $schedulePackages) { throw 'Restore the application packages before running the schedule tests.' }
& dotnet build (Join-Path $PSScriptRoot 'ScheduleTests.csproj') "-p:SQLitePackageRoot=$schedulePackages" --verbosity quiet
if ($LASTEXITCODE -ne 0) { throw "Schedule test build failed with exit code $LASTEXITCODE." }
& dotnet (Join-Path $PSScriptRoot 'bin/Debug/net8.0/ScheduleTests.dll')
if ($LASTEXITCODE -ne 0) { throw "Schedule tests failed with exit code $LASTEXITCODE." }
