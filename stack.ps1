param(
    [ValidateSet("local", "server")]
    [string]$Target = "local",

    [ValidateSet("up", "down", "restart", "status", "logs", "config", "build", "export", "load")]
    [string]$Action = "up",

    [string]$BundlePath = ""
)

$ErrorActionPreference = "Stop"
$projectRoot = $PSScriptRoot
$baseCompose = Join-Path $projectRoot "docker-compose.yml"
$localCompose = Join-Path $projectRoot "docker-compose.local.yml"

if (-not (Get-Command docker -ErrorAction SilentlyContinue)) {
    throw "Docker CLI not found. Install Docker Desktop and try again."
}

if ($Action -eq "load") {
    $bundle = if ($BundlePath) { [IO.Path]::GetFullPath($BundlePath) } else { $projectRoot }
    $checksums = Join-Path $bundle "SHA256SUMS"
    if (-not (Test-Path $checksums)) { throw "Missing SHA256SUMS in $bundle" }
    foreach ($line in Get-Content $checksums) {
        if ($line -notmatch '^([a-f0-9]{64})  ([a-zA-Z0-9._-]+)$') { throw "Invalid checksum record" }
        $expected = $Matches[1]
        $file = Join-Path $bundle $Matches[2]
        if ((Get-FileHash -LiteralPath $file -Algorithm SHA256).Hash.ToLowerInvariant() -ne $expected) {
            throw "Checksum mismatch: $file"
        }
    }
    & docker image load --input (Join-Path $bundle "images.tar.gz")
    if ($LASTEXITCODE -ne 0) { throw "Image import failed" }
    exit 0
}

function Initialize-GameSubmodules {
    if (-not (Get-Command git -ErrorAction SilentlyContinue)) {
        throw "Git CLI not found. It is required to initialize the game submodules."
    }

    if (-not (Test-Path (Join-Path $projectRoot ".gitmodules"))) {
        throw "Missing .gitmodules. Clone LingvoGameOs from Git instead of deploying a plain git archive."
    }

    & git -C $projectRoot rev-parse --is-inside-work-tree *> $null
    if ($LASTEXITCODE -ne 0) {
        throw "LingvoGameOs is not a Git checkout. Clone it with --recurse-submodules."
    }

    & git -C $projectRoot submodule sync --recursive
    if ($LASTEXITCODE -ne 0) {
        throw "Failed to synchronize game submodule URLs."
    }

    & git -C $projectRoot submodule update --init --recursive
    if ($LASTEXITCODE -ne 0) {
        throw "Failed to checkout the pinned game versions. Check Git access to every game repository."
    }

    if ($Target -eq "server") {
        & git -C $projectRoot submodule foreach --quiet --recursive 'test -z "$(git status --porcelain)"'
        if ($LASTEXITCODE -ne 0) {
            throw "Server deploy refused: at least one game submodule contains uncommitted changes."
        }
    }
}

$envFile = Join-Path $projectRoot ".env"
if (-not (Test-Path $envFile)) {
    $envTemplate = if ($Target -eq "local") { ".env.local.example" } else { ".env.server.example" }
    throw "Create .env from $envTemplate and fill the required settings."
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

if ($Target -eq "server" -and $Action -eq "up" -and (Test-Path (Join-Path $projectRoot "IMAGES"))) {
    $needsImport = $false
    foreach ($line in Get-Content (Join-Path $projectRoot "IMAGES")) {
        $expectedId, $image = $line -split ' ', 2
        $currentId = & docker image inspect --format '{{.Id}}' $image 2>$null
        if ($LASTEXITCODE -ne 0 -or $currentId -ne $expectedId) { $needsImport = $true }
    }
    if ($needsImport) {
        & (Join-Path $projectRoot "stack.ps1") server load $projectRoot
        if ($LASTEXITCODE -ne 0) { throw "Offline image import failed" }
    }
}

if (($Target -eq "local" -and $Action -in @("up", "build")) -or $Action -eq "build") {
    Initialize-GameSubmodules
}

switch ($Action) {
    "up" {
        Invoke-Compose config --quiet
        if ($Target -eq "server") {
            Invoke-Compose up -d --no-build --pull never --wait --remove-orphans
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
    "export" {
        $output = if ($BundlePath) { $BundlePath } else { Join-Path $projectRoot "artifacts/offline/$(Get-Date -Format yyyyMMdd-HHmmss)" }
        & python (Join-Path $projectRoot "deploy/bundle.py") --output $output
        if ($LASTEXITCODE -ne 0) { throw "Offline export failed" }
    }
}
