# TeddyBench v1.7.0-linux.1 - experimental Linux Mint adaptation

Unofficial AI-assisted fork of the upstream 1.7.0 code line, based on upstream master at 03e1d6e. This is not an official upstream release.

## Included

- Linux cross-build configuration for the net48 Windows Forms GUI.
- Mono launcher and packaged GUI output.
- Explicit framework references and resource-generation fixes.
- Restored ScottPlot.WinForms and explicit LINQ Reverse calls.
- New Linux MessageBox replacement using normal modal forms and readable standard buttons; Windows delegates to the original MessageBox.
- English documentation and SHA256 checksum.

## Test status

Linux Mint 22.3 Cinnamon with Mono 6.8: launch, manually opening CONTENT, listing Tonies, and OGG export were reported working on the earlier build. One exported OGG passed an FFmpeg decoding check. The independent custom-dialog proof of concept displayed its buttons correctly. The integrated dialog replacement has not yet been confirmed on Mint. CI success is a compilation check, not a runtime certification.

## Known limitations

Scrolling/repaint artifacts remain. An Ogg-page-header warning remains under investigation. NAudio is still referenced by TonieAudio; a full FFmpeg port has not been merged. Import, playback, RFID hardware, firmware flashing, and all-export completeness are not validated. The Linux dialog wrapper currently omits message icons and MessageBoxOptions behavior.

## Installation

Install mono-complete, libgdiplus, and ffmpeg using your package manager. Download the tar.xz and matching .sha256 asset, verify with sha256sum -c, extract, and run ./TeddyBench from the extracted directory as your ordinary user. Select the SD card's CONTENT directory. See the packaged README for complete instructions and troubleshooting. Back up the card first.
