"""Build/export a complete, secret-free installation for an offline server."""
import argparse
import gzip
import hashlib
import json
import os
from pathlib import Path
import shutil
import subprocess

ROOT = Path(__file__).resolve().parents[1]


def run(*args, capture=False):
    result = subprocess.run(args, check=True, text=True, stdout=subprocess.PIPE if capture else None)
    return result.stdout if capture else None


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--output", type=Path, required=True)
    parser.add_argument("--platform", default="linux/amd64")
    parser.add_argument("--env-file", type=Path, default=ROOT / ".env")
    args = parser.parse_args()
    output = args.output.resolve()
    if output.exists():
        parser.error("Output already exists; select a new release directory")
    if not args.env_file.is_file():
        parser.error("Create and fill the working .env before building")
    os.environ["DOCKER_DEFAULT_PLATFORM"] = args.platform
    compose = ["docker", "compose", "--project-directory", str(ROOT), "--env-file", str(args.env_file.resolve()),
               "-f", str(ROOT / "docker-compose.yml")]
    # Use the current checked-out source tree, including reviewed local changes.
    # The wrapper initializes submodules beforehand; it never fetches unpinned HEADs.
    run(*compose, "config", "--quiet")
    run(*compose, "build")
    run(*compose, "pull", "postgres")
    resolved = json.loads(run(*compose, "config", "--format", "json", capture=True))
    template = json.loads(run(*compose, "config", "--no-interpolate", "--format", "json", capture=True))
    output.mkdir(parents=True)
    images = []
    for name, service in template["services"].items():
        image = resolved["services"][name]["image"]
        service.pop("build", None)
        service["image"] = image
        service["pull_policy"] = "never"
        service["platform"] = args.platform
        images.append(image)
    # Compose normalizes relative bind mounts against the build machine.
    # Keep the PostgreSQL data location relative to the destination server.
    template["services"]["postgres"]["volumes"][0] = {
        "type": "bind", "source": "${POSTGRES_DATA_PATH:-./postgres-db}",
        "target": "/var/lib/postgresql/data", "bind": {"create_host_path": True}
    }
    # `compose config` expands automatically generated resource names using the
    # builder's project name. Let the destination select its own isolated names.
    for volume in template.get("volumes", {}).values():
        if not volume.get("external"):
            volume.pop("name", None)
    for network in template.get("networks", {}).values():
        if not network.get("external"):
            network.pop("name", None)
    template.pop("x-game-environment", None)
    template.pop("x-game-service", None)
    (output / "docker-compose.yml").write_text(json.dumps(template, ensure_ascii=False, indent=2), encoding="utf-8")
    run("docker", "compose", "--project-directory", str(output), "--env-file", str(args.env_file.resolve()),
        "-f", str(output / "docker-compose.yml"), "config", "--quiet")
    records = []
    for image in dict.fromkeys(images):
        info = json.loads(run("docker", "image", "inspect", image, capture=True))[0]
        records.append({"image": image, "id": info["Id"], "platform": args.platform})
    # Stream directly into gzip, avoiding a second multi-gigabyte temporary tar.
    with subprocess.Popen(["docker", "image", "save", *dict.fromkeys(images)], stdout=subprocess.PIPE) as exporter:
        with gzip.open(output / "images.tar.gz", "wb", compresslevel=3) as target:
            shutil.copyfileobj(exporter.stdout, target)
        if exporter.wait() != 0:
            raise RuntimeError("docker image save failed")
    for filename in ("stack.ps1", "stack.sh", ".env.server.example"):
        shutil.copyfile(ROOT / filename, output / filename)
    (output / "release.json").write_text(json.dumps({"platform": args.platform, "images": records}, indent=2), encoding="utf-8")
    (output / "IMAGES").write_text("".join(f"{record['id']} {record['image']}\n" for record in records), encoding="utf-8", newline="\n")
    lines = []
    for filename in ("images.tar.gz", "docker-compose.yml", "stack.ps1", "stack.sh", ".env.server.example", "release.json", "IMAGES"):
        with (output / filename).open("rb") as stream:
            digest = hashlib.file_digest(stream, "sha256").hexdigest()
        lines.append(f"{digest}  {filename}\n")
    (output / "SHA256SUMS").write_text("".join(lines), encoding="utf-8", newline="\n")
    print(f"Offline release: {output}; {len(records)} images. No working .env, Git checkout or sources included.")


if __name__ == "__main__":
    main()
