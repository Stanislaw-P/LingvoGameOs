"""Download pinned browser dependencies into Web's offline static directory.

Run only on a build machine with Internet. The resulting files are copied into
the Web image; neither the server nor a player's browser needs an external CDN.
"""
import hashlib
import io
import json
from pathlib import Path
import tarfile
import urllib.request

ROOT = Path(__file__).resolve().parents[1] / "LingvoGameOsWeb" / "wwwroot" / "vendor"
PACKAGES = [
    ("bootstrap", "5.3.3", "bootstrap5", ("dist/css/", "dist/js/", "LICENSE")),
    ("bootstrap", "4.4.1", "bootstrap4", ("dist/css/", "LICENSE")),
    ("bootstrap-icons", "1.11.0", "bootstrap-icons", ("font/", "LICENSE")),
    ("@fortawesome/fontawesome-free", "6.4.0", "fontawesome", ("css/", "webfonts/", "LICENSE.txt")),
    ("@tailwindcss/browser", "4.1.14", "tailwind", ("dist/index.global.js", "LICENSE")),
    ("@fontsource/press-start-2p", "5.2.5", "press-start-2p", ("files/", "400.css", "LICENSE")),
    ("@fontsource/alegreya-sc", "5.2.5", "alegreya-sc", ("files/", "400.css", "500.css", "700.css", "800.css", "900.css", "LICENSE")),
    ("@fontsource/dela-gothic-one", "5.2.5", "dela-gothic-one", ("files/", "400.css", "LICENSE")),
]


def main():
    manifest = []
    for package, version, directory, selected in PACKAGES:
        metadata = json.load(urllib.request.urlopen(f"https://registry.npmjs.org/{package}/{version}", timeout=30))
        payload = urllib.request.urlopen(metadata["dist"]["tarball"], timeout=60).read()
        expected = metadata["dist"]["shasum"]
        if hashlib.sha1(payload).hexdigest() != expected:
            raise ValueError(f"Package checksum mismatch: {package}")
        count = 0
        with tarfile.open(fileobj=io.BytesIO(payload), mode="r:gz") as archive:
            for entry in archive:
                relative = entry.name.removeprefix("package/")
                if not entry.isfile() or not any(relative.startswith(prefix) for prefix in selected):
                    continue
                if ".." in Path(relative).parts or Path(relative).is_absolute():
                    raise ValueError("Invalid package path")
                destination = ROOT / directory / relative
                destination.parent.mkdir(parents=True, exist_ok=True)
                destination.write_bytes(archive.extractfile(entry).read())
                count += 1
        manifest.append({"package": package, "version": version, "sha1": expected, "files": count})
        print(f"{package}@{version}: {count} files")
    (ROOT / "manifest.json").write_text(json.dumps(manifest, indent=2), encoding="utf-8")


if __name__ == "__main__":
    main()
