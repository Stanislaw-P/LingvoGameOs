#!/usr/bin/env bash
set -Eeuo pipefail

target="${1:-local}"
action="${2:-up}"
project_root="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)"

case "$target" in
  local|server) ;;
  *) echo "Usage: bash ./stack.sh <local|server> <up|down|restart|status|logs|config|build>" >&2; exit 2 ;;
esac

case "$action" in
  up|down|restart|status|logs|config|build) ;;
  *) echo "Usage: bash ./stack.sh <local|server> <up|down|restart|status|logs|config|build>" >&2; exit 2 ;;
esac

command -v docker >/dev/null 2>&1 || {
  echo "Docker CLI not found." >&2
  exit 1
}

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

if [[ "$target" == "local" ]]; then
  if [[ -f "$project_root/.env.local" ]]; then
    env_file="$project_root/.env.local"
  else
    env_file="$project_root/.env.local.example"
  fi
else
  if [[ -f "$project_root/.env.server" ]]; then
    env_file="$project_root/.env.server"
  elif [[ -f "$project_root/.env" ]]; then
    env_file="$project_root/.env"
  else
    echo "Create .env.server from .env.server.example and fill the server secrets." >&2
    exit 1
  fi
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

case "$action" in
  up|build|config) prepare_game_submodules ;;
esac

case "$action" in
  up)
    "${compose[@]}" config --quiet
    if [[ "$target" == "server" ]]; then
      "${compose[@]}" build --pull
      "${compose[@]}" up -d --wait --remove-orphans
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
esac
