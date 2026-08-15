$ErrorActionPreference = "Stop"

function Invoke-BuildStep {
	param(
		[string]$Name,
		[scriptblock]$Action
	)

	Write-Host "`n=== $Name ===" -ForegroundColor Cyan
	& $Action
	if ($LASTEXITCODE -ne 0) {
		throw "$Name failed with exit code $LASTEXITCODE."
	}
}

$AspNetRoot = Split-Path $PSScriptRoot -Parent
$RepoRoot = Split-Path $AspNetRoot -Parent
$ReleaseRoot = Join-Path $AspNetRoot "release"
$MigratorRelease = Join-Path $ReleaseRoot "migrator"
$WebHostRelease = Join-Path $ReleaseRoot "webhost"
$AngularRelease = Join-Path $ReleaseRoot "angular"

try {
	foreach ($Command in @("dotnet", "yarn.cmd")) {
		if (-not (Get-Command $Command -ErrorAction SilentlyContinue)) {
			throw "Required command '$Command' was not found in PATH."
		}
	}

	Invoke-BuildStep "Publishing Migrator" {
		dotnet publish (Join-Path $AspNetRoot "src/PawnCloud.Migrator/PawnCloud.Migrator.csproj") -c Release -o $MigratorRelease
	}

	Invoke-BuildStep "Publishing Web.Host" {
		dotnet publish (Join-Path $AspNetRoot "src/PawnCloud.Web.Host/PawnCloud.Web.Host.csproj") -c Release -o $WebHostRelease
	}

	Push-Location (Join-Path $RepoRoot "angular")
	try {
		Invoke-BuildStep "Installing Angular dependencies" {
			yarn.cmd install
		}

		Invoke-BuildStep "Building Angular production bundle" {
			yarn.cmd run ng build --configuration production
		}
	}
	finally {
		Pop-Location
	}

	Invoke-BuildStep "Preparing Angular deployment folder" {
		New-Item -ItemType Directory -Force $AngularRelease | Out-Null
		Copy-Item (Join-Path $RepoRoot "angular/dist/browser/*") $AngularRelease -Recurse -Force
		Copy-Item (Join-Path $RepoRoot "angular/Dockerfile") $AngularRelease -Force
		Copy-Item (Join-Path $RepoRoot "angular/fast-nginx-default.conf") $AngularRelease -Force
	}

	Write-Host "`nDeployment folders are ready:" -ForegroundColor Green
	Write-Host $MigratorRelease
	Write-Host $WebHostRelease
	Write-Host $AngularRelease
}
catch {
	Write-Host "`nBUILD FAILED" -ForegroundColor Red
	Write-Error $_
	exit 1
}
