param(
    [ValidateSet("local", "server")]
    [string]$Target = "local",

    [ValidateSet("up", "down", "restart", "status", "logs", "config", "build")]
    [string]$Action = "up"
)

$ErrorActionPreference = "Stop"
$projectRoot = $PSScriptRoot
$baseCompose = Join-Path $projectRoot "docker-compose.yml"
$localCompose = Join-Path $projectRoot "docker-compose.local.yml"

if (-not (Get-Command docker -ErrorAction SilentlyContinue)) {
    throw "Docker CLI not found. Install Docker Desktop and try again."
}

if ($Target -eq "local") {
    $localEnv = Join-Path $projectRoot ".env.local"
    $envFile = if (Test-Path $localEnv) {
        $localEnv
    } else {
        Join-Path $projectRoot ".env.local.example"
    }
} else {
    $serverEnv = Join-Path $projectRoot ".env.server"
    $legacyEnv = Join-Path $projectRoot ".env"
    if (Test-Path $serverEnv) {
        $envFile = $serverEnv
    } elseif (Test-Path $legacyEnv) {
        $envFile = $legacyEnv
    } else {
        throw "Create .env.server from .env.server.example and fill the server secrets."
    }
}

$composeArgs = @(
    "compose",
    "--project-directory", $projectRoot,
    "--env-file", $envFile,
    "-f", $baseCompose
)
if ($Target -eq "local") {
    $composeArgs += @("-f", $localCompose)
}

function Invoke-Compose {
    param([Parameter(ValueFromRemainingArguments = $true)][string[]]$Arguments)
    & docker @composeArgs @Arguments
    if ($LASTEXITCODE -ne 0) {
        throw "docker compose failed with exit code $LASTEXITCODE"
    }
}

Write-Host "LingvoGameOs: target=$Target, action=$Action, env=$(Split-Path -Leaf $envFile)"

switch ($Action) {
    "up" {
        Invoke-Compose config --quiet
        if ($Target -eq "server") {
            Invoke-Compose build --pull
            Invoke-Compose up -d --wait --remove-orphans
        } else {
            Invoke-Compose up -d --build --wait --remove-orphans
        }
        Invoke-Compose ps
    }
    "down" {
        Invoke-Compose down --remove-orphans
    }
    "restart" {
        Invoke-Compose restart
        Invoke-Compose ps
    }
    "status" {
        Invoke-Compose ps
    }
    "logs" {
        Invoke-Compose logs --tail 200 -f
    }
    "config" {
        Invoke-Compose config --quiet
        Write-Host "Compose configuration is valid."
    }
    "build" {
        if ($Target -eq "server") {
            Invoke-Compose build --pull
        } else {
            Invoke-Compose build
        }
    }
}
