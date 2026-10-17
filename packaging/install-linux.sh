#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
INSTALL_DIR="/opt/teddybench"
DESKTOP_DIR="/usr/share/applications"

if [[ $EUID -ne 0 ]]; then
  echo "Please run this installer with sudo:"
  echo "  sudo ./install-linux.sh"
  exit 1
fi

if [[ ! -x "$SCRIPT_DIR/TeddyBench" ]]; then
  echo "Error: TeddyBench was not found beside this installer."
  echo "Expected: $SCRIPT_DIR/TeddyBench"
  exit 1
fi

sudo mkdir -p "$INSTALL_DIR"
sudo cp -a "$SCRIPT_DIR/." "$INSTALL_DIR/"
sudo chmod +x "$INSTALL_DIR/TeddyBench"

if [[ -f "$INSTALL_DIR/dog.ico" ]]; then
  if command -v convert >/dev/null 2>&1; then
    convert "$INSTALL_DIR/dog.ico[0]" "$INSTALL_DIR/teddybench.png" || true
  fi
fi

if [[ ! -f "$INSTALL_DIR/teddybench.png" ]]; then
  sudo cp "$INSTALL_DIR/dog.ico" "$INSTALL_DIR/teddybench.ico" 2>/dev/null || true
fi

sudo cp "$SCRIPT_DIR/teddybench.desktop" "$DESKTOP_DIR/teddybench.desktop"

if command -v update-desktop-database >/dev/null 2>&1; then
  update-desktop-database "$DESKTOP_DIR" || true
fi

echo "TeddyBench was installed to $INSTALL_DIR."
echo "Look for TeddyBench in the Linux Mint application menu."
