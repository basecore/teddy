# Tonie file tool

With this tool you can dump existing files of the famous audio box or create custom ones.

## TeddyBench for Linux - unofficial fork

Maintained by basecore with AI assistance. Original author credits and license notices remain applicable. The maintained Linux source branch is `linux-1.7.0`.

### Installer release v1.7.0-linux.2

Download `TeddyBench-Linux-1.7.0-linux.2.zip` and its `.sha256` file from:

https://github.com/basecore/teddy/releases/tag/v1.7.0-linux.2

Use the release asset, not GitHub's automatically generated source-code archives. The release is available after the release workflow completes successfully.

This installer release reuses the application binary from `v1.7.0-linux.1`; subsequent UI source commits are not included. The installer was successfully tested by the maintainer on Linux Mint. This does not certify every application or hardware function.

### Install

Download both files to Downloads. Extract to a fresh folder:

```bash
cd ~/Downloads
sha256sum -c TeddyBench-Linux-1.7.0-linux.2.zip.sha256
TESTDIR=$(mktemp -d "$HOME/Downloads/teddybench-install.XXXXXX")
unzip TeddyBench-Linux-1.7.0-linux.2.zip -d "$TESTDIR"
cd "$TESTDIR/TeddyBench-Linux"
sudo bash ./install-linux.sh
```

Then search for TeddyBench in the Linux Mint application menu. Launch the application as your normal user, not with sudo. You can add the menu entry to your favorites.

The installer copies the application to `/opt/teddybench` and creates `/usr/share/applications/teddybench.desktop`. The original `dog.ico` and a PNG generated during packaging are included. ImageMagick is not required on the user's machine.

Run the installer from the freshly extracted directory, never from `/opt/teddybench` itself. The installer does not install runtime dependencies. This is the Mono-based Linux version, not Avalonia; the .NET SDK is not required for this prebuilt package. A compatible Mono runtime and libgdiplus must already be available.

### Uninstall

```bash
sudo bash /opt/teddybench/uninstall-linux.sh
```

This removes the application installation and its system menu entry.

### Actions artifacts versus releases

Actions artifact downloads may wrap the application ZIP in another ZIP. A direct release-asset download contains `TeddyBench-Linux` after a single extraction.
