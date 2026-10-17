# TeddyBench for Linux Mint

An unofficial, AI-assisted Linux adaptation of TeddyBench from [toniebox-reverse-engineering/teddy](https://github.com/toniebox-reverse-engineering/teddy), developed and tested interactively by basecore with assistance from Perplexity AI.

## Status and scope

This fork adapts the upstream TeddyBench 1.7.0 code line for building on Linux and running with Mono. It is not an official upstream release, a native GTK application, or a complete rewrite. The working branch is `linux-1.7.0`.

Important provenance detail: this branch was created from upstream master at `03e1d6efffbf0353ea9899b901942f7aa28c49e3`, not directly from the v1.7.0 tag (`ca062fee5f8da450468e466b1a36e658deb56827`). It therefore includes the master changes present at that point. The upstream v1.7.0 release remains the functional baseline, not an assertion of byte-identical sources.

The older [AlbrechtL Linux fork](https://github.com/AlbrechtL/teddy/releases/tag/v1.2.4-linux_2) informed the launcher approach. Its full FFmpeg audio port has NOT been merged into this fork. Original authors retain credit for their code; AI assistance does not imply upstream endorsement or verified correctness.

## Testing actually performed

User testing was performed on Linux Mint 22.3 Zena, Cinnamon, with Mono 6.8.0.105, libgdiplus 6.1, Cairo 1.18.0, and Pango 1.52.1.

Confirmed in the interactive test session:

- The Linux CI build completed with zero errors before the latest dialog replacement.
- The application launched on Linux Mint.
- Manually opening the mounted SD card's CONTENT directory displayed the stored Tonies and artwork.
- Audio export produced OGG files.
- FFmpeg decoded one selected exported OGG without reporting errors.
- An independent custom modal dialog displayed readable buttons, whereas Mono's standard MessageBox did not.

These are limited functional tests, not a full compatibility certification. No automated GUI validation, complete audio comparison, RFID reader test, firmware flashing test, or comprehensive import/playback test has been completed. The actual TeddyBench integration of the new dialog replacement still requires confirmation on Mint. A successful build does not prove every feature works.

## Download and install

Use the Assets section of this fork's [Releases page](https://github.com/basecore/teddy/releases). The first Linux release is planned as `v1.7.0-linux.1`, marked as a pre-release because known issues remain. Availability depends on successful completion of the release workflow.

Download `TeddyBench-Linux-1.7.0-linux.1.tar.xz` and its `.sha256` file into the same directory. Do not use GitHub's automatically generated Source code archives as the runnable application.

Install runtime dependencies on Mint:

```bash
sudo apt update
sudo apt install mono-complete libgdiplus ffmpeg
```

Verify and extract:

```bash
cd ~/Downloads
sha256sum -c TeddyBench-Linux-1.7.0-linux.1.tar.xz.sha256
mkdir -p ~/Applications/teddybench-linux-1
 tar -xf TeddyBench-Linux-1.7.0-linux.1.tar.xz -C ~/Applications/teddybench-linux-1
cd ~/Applications/teddybench-linux-1/TeddyBench-Linux
chmod +x TeddyBench
./TeddyBench
```

Run as your ordinary user, not with sudo. A graphical desktop is required. The launcher must remain next to TeddyBench.exe and the packaged runtime files.

Actions artifacts are an alternative download mechanism. GitHub wraps an artifact in a ZIP; extract that ZIP first, then extract the tar.xz inside it. They are two separate archive layers.

## How this runs on Linux

The GUI remains a .NET Framework 4.8 Windows Forms application. The .NET SDK compiles it; Mono executes the resulting managed TeddyBench.exe on Linux. The .exe extension does not mean Wine is used. Mono implements the Windows Forms API, while libgdiplus provides System.Drawing functionality using the Linux graphics stack. The host fonts and graphics libraries therefore affect rendering.

The launcher checks for Mono and FFmpeg, sets MONO_MANAGED_WATCHER=disabled and MONO_WINFORMS_XIM_STYLE=disabled, changes to its own directory, and executes `mono TeddyBench.exe`. FFmpeg is a required launcher dependency and a useful validation tool; installing it alone does not replace NAudio calls.

## Changes made in this fork

### Project and build configuration

- Removed the GUI project's Windows runtime identifier and single-file publish settings.
- Replaced implicit Windows desktop targeting in the GUI project with explicit framework references including System.Windows.Forms and System.Drawing.
- Kept the GUI target at net48; helper libraries still include netstandard2.0 targets.
- Added Microsoft.NETFramework.ReferenceAssemblies through Directory.Build.props so reference assemblies can be restored for cross-platform compilation.
- Enabled GenerateResourceUsePreserializedResources and added System.Resources.Extensions for GUI resources when building with the .NET SDK.
- Added System.IO.Ports as a package reference rather than an unresolved standalone assembly reference.
- Restored ScottPlot.WinForms 4.1.28, required by ScottPlot.FormsPlot in the antenna form.
- Made the three failing Proxmark3 reverse operations explicitly use Enumerable.Reverse(...).ToArray() to preserve reversed-copy semantics without ambiguous extension-method binding.

### Linux execution and packaging

- Added the Linux/TeddyBench launcher.
- Added Linux CI using Ubuntu 22.04 and dotnet restore/build, rather than deprecated xbuild, which cannot build these SDK-style project files.
- Added archive packaging for application output and the launcher.
- Added a release workflow with a pinned .NET 8 SDK selection, persistent build logs, a SHA256 checksum, and GitHub release publication only after a successful build.

### Standard dialog workaround

TeddyBench/MessageBox.cs supplies a same-namespace wrapper for matching unqualified MessageBox.Show calls. On Linux it creates a regular modal Form with ordinary Buttons instead of Mono's standard MessageBox. On Windows it delegates to System.Windows.Forms.MessageBox.

It supports OK, OKCancel, YesNo, YesNoCancel, RetryCancel, and AbortRetryIgnore, preserving their DialogResult values. Cancel-capable dialogs provide a cancel action; OK-only dialogs can close as OK. Dialogs without a cancel action disable the window close control. Default-button selection is retained. A supplied Control owner is used for UI-thread marshaling when required.

The replacement currently uses English labels, does not display the requested MessageBox icon, and does not implement MessageBoxOptions on Linux. It is not a complete drop-in implementation of every Windows MessageBox overload. Fully qualified System.Windows.Forms.MessageBox calls bypass it. Build and runtime tests remain necessary. Scrolling artifacts are not addressed by this change.

## Opening your SD card

Back up the card before modifying it. Insert it and open it in Mint's file manager. In TeddyBench's folder chooser, select the CONTENT directory itself, not just the device root. This was the successful workaround for the tested card. Automatic removable-drive discovery is not yet validated on Linux.

Do not format the card, rename its content folders, or run TeddyBench as root to solve detection problems. Keep the initial tests read-only apart from exporting to a separate host directory.

## Known issues and troubleshooting

### Blank standard-dialog buttons

The tested Mono installation had invisible standard MessageBox button labels even though their Text values were present. Locale changes, font replacement, and both compatible text-rendering settings did not fix the independent reproduction. A custom modal form did work. The new wrapper applies that workaround, but its integration must still be tested in TeddyBench.

### Scroll/repaint corruption

The Tonie list showed fragmented text and artwork when scrolling or returning to the window. This remains unresolved. Do not interpret successful compilation as a fix for this issue.

### Pango warnings

Warnings included `failed to create cairo scaled font` and `scaled_font status is: out of memory`. These messages did not establish physical RAM exhaustion. Testing PANGOCAIRO_BACKEND=fc did not fix the visible dialog problem. No global font changes are required by this fork.

### OGG reader warning

A test session reported `Not an Ogg page header at 0x01F57000` while exports were still produced. The exact originating file and cause have not been established. One exported track passed decoding validation, which does not prove all tracks are complete.

```bash
ffmpeg -v error -i '/path/to/exported-track.ogg' -f null -
```

Also compare duration, first/last audio, and expected track count. Retain the original card backup.

### Unknown titles

An entry may appear as Unnamed Teddy or unknown even when its audio exports. The metadata database does not necessarily identify every content variant. Do not delete or rename original SD files merely to change a displayed title.

### Remaining NAudio dependency

TonieAudio still references NAudio 2.0.1. NU1701 reports a framework compatibility fallback for NAudio.WinForms. This is a real compatibility limitation, not evidence of a completed FFmpeg conversion. Playback, import, conversion, and device-specific paths need separate validation.

### Other build warnings

GitInfo warnings affect version metadata and need investigation if version strings are inaccurate. Unused-field and unreachable-code warnings remain. Costura.Fody reported an IncludeAssets recommendation. These warnings did not block the reported successful build, but should not all be dismissed as harmless.

Record runtime output without truncating it:

```bash
./TeddyBench 2>&1 | tee ~/teddybench-runtime.log
```

Do not pipe a long-running GUI to head when collecting a complete log. Remove personal paths, card UIDs, and sensitive data before posting logs publicly.

## Building from source

Install the .NET 8 SDK for compilation and Mono/libgdiplus/FFmpeg for execution. Clone the Linux branch:

```bash
git clone --branch linux-1.7.0 https://github.com/basecore/teddy.git
cd teddy
dotnet restore Teddy.sln
dotnet build Teddy.sln -c Release -p:Platform="Any CPU" --no-restore
cp Linux/TeddyBench TeddyBench/bin/Release/net48/TeddyBench
cd TeddyBench/bin/Release/net48
chmod +x TeddyBench
./TeddyBench
```

The build output is TeddyBench/bin/Release/net48. On machines with multiple SDKs, use the release workflow's SDK selection approach for consistent results. This builds managed assemblies; it does not produce a self-contained native executable or AppImage.

## Release process

.github/workflows/release-linux.yml builds the branch snapshot containing this documentation, packages the complete GUI output directory with the launcher, README, and release notes, creates a checksum, and publishes the public pre-release v1.7.0-linux.1. It refuses to overwrite an existing release. The initial publication is triggered by pushing that workflow file on linux-1.7.0; subsequent documentation edits do not automatically republish it. Publication requires the repository to allow GitHub Actions and a contents-write GITHUB_TOKEN.

GitHub adds source ZIP/tar.gz links automatically. The runnable tar.xz is a separate release asset. Pre-releases may not be shown as the stable Latest release. The default branch is not changed by this work; select linux-1.7.0 to view this README if the repository home page still shows the original README.

## Credits and responsible use

Original project: toniebox-reverse-engineering/teddy and its contributors, including g3gg0. Historical Linux work: AlbrechtL/teddy. Linux adaptation and interactive tests: basecore, assisted by Perplexity AI. AI-generated changes require human review and testing. Existing source notices and third-party licenses remain applicable; this README does not grant a new license or change ownership. Use only content you are entitled to access and retain backups before any write operation.
