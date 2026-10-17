#!/usr/bin/env bash
set -euo pipefail

INSTALL_DIR="/opt/teddybench"
DESKTOP_FILE="/usr/share/applications/teddybench.desktop"

if [[ $EUID -ne 0 ]]; then
  echo "Please run this uninstaller with sudo:"
  echo "  sudo ./uninstall-linux.sh"
  exit 1
fi

sudo rm -rf "$INSTALL_DIR"
sudo rm -f "$DESKTOP_FILE"

if command -v update-desktop-database >/dev/null 2>&1; then
  update-desktop-database "$(dirname "$DESKTOP_FILE")" || true
fi

echo "TeddyBench was removed."
