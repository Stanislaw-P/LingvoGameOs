#!/usr/bin/env bash
set -Eeuo pipefail

target="${1:-local}"
action="${2:-up}"
bundle_path="${3:-}"
project_root="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)"

case "$target" in
  local|server) ;;
  *) echo "Usage: bash ./stack.sh <local|server> <up|down|restart|status|logs|config|build>" >&2; exit 2 ;;
esac

case "$action" in
  up|down|restart|status|logs|config|build|export|load) ;;
  *) echo "Usage: bash ./stack.sh <local|server> <up|down|restart|status|logs|config|build>" >&2; exit 2 ;;
esac

command -v docker >/dev/null 2>&1 || {
  echo "Docker CLI not found." >&2
  exit 1
}

if [[ "$action" == "load" ]]; then
  bundle="${bundle_path:-$project_root}"
  ( cd -- "$bundle"; sha256sum --check SHA256SUMS; docker image load --input images.tar.gz )
  exit 0
fi

prepare_game_submodules() {
  command -v git >/dev/null 2>&1 || {
    echo "Git CLI not found. It is required to initialize the game submodules." >&2
    exit 1
  }

  [[ -f "$project_root/.gitmodules" ]] || {
    echo "Missing .gitmodules. Clone LingvoGameOs from Git instead of deploying a plain git archive." >&2
    exit 1
  }

  git -C "$project_root" rev-parse --is-inside-work-tree >/dev/null 2>&1 || {
    echo "LingvoGameOs is not a Git checkout. Clone it with --recurse-submodules." >&2
    exit 1
  }

  git -C "$project_root" submodule sync --recursive
  git -C "$project_root" submodule update --init --recursive || {
    echo "Failed to checkout the pinned game versions. Check Git access to every game repository." >&2
    exit 1
  }

  if [[ "$target" == "server" ]] &&
    ! git -C "$project_root" submodule foreach --quiet --recursive \
      'test -z "$(git status --porcelain)"'; then
    echo "Server deploy refused: at least one game submodule contains uncommitted changes." >&2
    exit 1
  fi
}

env_file="$project_root/.env"
if [[ ! -f "$env_file" ]]; then
  if [[ "$target" == "local" ]]; then
    env_template=".env.local.example"
  else
    env_template=".env.server.example"
  fi
  echo "Create .env from $env_template and fill the required settings." >&2
  exit 1
fi

compose=(
  docker compose
  --project-directory "$project_root"
  --env-file "$env_file"
  -f "$project_root/docker-compose.yml"
)
if [[ "$target" == "local" ]]; then
  compose+=(-f "$project_root/docker-compose.local.yml")
fi

echo "LingvoGameOs: target=$target, action=$action, env=$(basename "$env_file")"

if [[ "$target" == "server" && "$action" == "up" && -f "$project_root/IMAGES" ]]; then
  needs_import=false
  while read -r expected_id image; do
    current_id="$(docker image inspect --format '{{.Id}}' "$image" 2>/dev/null || true)"
    [[ "$current_id" == "$expected_id" ]] || needs_import=true
  done < "$project_root/IMAGES"
  if [[ "$needs_import" == "true" ]]; then
    bash "$project_root/stack.sh" server load "$project_root"
  fi
fi

if [[ "$action" == "build" || ( "$target" == "local" && "$action" == "up" ) ]]; then
  prepare_game_submodules
fi

case "$action" in
  up)
    "${compose[@]}" config --quiet
    if [[ "$target" == "server" ]]; then
      "${compose[@]}" up -d --no-build --pull never --wait --remove-orphans
    else
      "${compose[@]}" up -d --build --wait --remove-orphans
    fi
    "${compose[@]}" ps
    ;;
  down)
    "${compose[@]}" down --remove-orphans
    ;;
  restart)
    "${compose[@]}" restart
    "${compose[@]}" ps
    ;;
  status)
    "${compose[@]}" ps
    ;;
  logs)
    "${compose[@]}" logs --tail 200 -f
    ;;
  config)
    "${compose[@]}" config --quiet
    echo "Compose configuration is valid."
    ;;
  build)
    if [[ "$target" == "server" ]]; then
      "${compose[@]}" build --pull
    else
      "${compose[@]}" build
    fi
    ;;
  export)
    python3 "$project_root/deploy/bundle.py" --output "${bundle_path:-$project_root/artifacts/offline/$(date +%Y%m%d-%H%M%S)}"
    ;;
esac
