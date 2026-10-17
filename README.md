# TeddyBench for Linux

Inoffizieller Linux-Fork von TeddyBench, gepflegt von basecore mit KI-Unterstützung. Ziel ist eine zuverlässige Linux-Version mit der vertrauten TeddyBench-Oberfläche, nicht eine zweite Anwendung mit anderem Bedienkonzept.

Das ursprüngliche Projekt enthält Werkzeuge zum Auslesen bestehender Tonie-Dateien und zum Erstellen eigener Dateien. Nicht alle ursprünglichen Funktionen sind bereits unter Linux getestet. Hinweise auf ursprüngliche Autoren und bestehende Lizenzbedingungen bleiben gültig; dieser Fork ist kein offizielles Upstream-Release.

## Aktueller Linux-Stand

- Linux-Quellcode: Branch `linux-1.7.0`.
- Installer-Version: `v1.7.0-linux.3`.
- Getesteter Anwendungscommit: `ac86aa22bde24e9f363fe8c44fbdd9ca54ac3336`.
- Oberfläche: weiterhin Windows Forms auf Mono mit libgdiplus, keine vollständig native GTK-/Qt-Anwendung.
- Linux-Wiedergabe: vorhandener Opus-Decoder mit PCM-Ausgabe an ffplay statt Windows-WaveOut/winmm.dll.

Der Linux-3-Releaseworkflow übernimmt das vorhandene erfolgreiche Build-Artifact dieses Commits, ohne die Anwendung neu zu kompilieren. BUILD-INFO.txt im Paket nennt Quellcommit, Build-Lauf und EXE-Prüfsumme.

Download: https://github.com/basecore/teddy/releases/tag/v1.7.0-linux.3

Die Dateien stehen erst nach erfolgreichem Abschluss des Releaseworkflows zur Verfügung. Die frühere Installer-Version linux.2 verwendet noch die alte linux.1-Binärdatei und enthält die neuen Wiedergabefixes nicht.

## Tests und Grenzen

Auf dem Linux-Mint-System des Maintainers bestätigt:

- Installation mit `sudo bash ./install-linux.sh`, Menüeintrag und Symbol beim vorherigen Installer-Test.
- CONTENT-Browsing und OGG-Export in bisherigen Tests.
- Wiedergabe im Testbuild ac86aa2.
- Sichtbare Play-/Stop-Beschriftung, auch nach Stoppen sowie Minimieren/Wiederherstellen.
- Doppelklick öffnet wieder den ursprünglichen Dialog "Enter tag UID".

Dies ist keine Zertifizierung aller Audiodateien, Schreib- oder Hardwarefunktionen. Das fertig paketierte Linux-3-Release benötigt zusätzlich einen Installationstest.

Bekannte Einschränkungen:

- Positionssuche und genaue Wiedergabezeitanzeige sind im ersten Linux-Audiobackend nicht implementiert.
- Teile der Oberfläche sind optisch uneinheitlich; Mono-Rendering benötigt Workarounds.
- NAudio und weitere Windows-orientierte Komponenten sind noch im Repository vorhanden. Nur die neue Linux-Wiedergabe umgeht WaveOut; eine vollständige Ablösung ist nicht abgeschlossen.
- NFC, Firmware-/Bootloader-Aktionen und weitere Hardwarefunktionen sind nicht umfassend unter Linux getestet.
- Ogg-Analysewarnungen können bei einzelnen Dateien auftreten und müssen separat untersucht werden.

## Voraussetzungen unter Linux Mint

Für das fertige Paket werden Mono, libgdiplus und FFmpeg einschließlich ffplay benötigt. Der Installer installiert diese Abhängigkeiten nicht automatisch. Kein .NET-SDK und kein ImageMagick sind zum Verwenden des Pakets nötig; die PNG-Symbolkonvertierung erfolgt auf GitHub.

Falls erforderlich:

```bash
sudo apt update
sudo apt install -y mono-complete libgdiplus ffmpeg curl unzip
```

## Manuelle Installation

TeddyBench vorher schließen. Von der Release-Seite diese beiden Assets herunterladen, nicht die automatisch erzeugten Source-code-Archive:

- `TeddyBench-Linux-1.7.0-linux.3.zip`
- `TeddyBench-Linux-1.7.0-linux.3.zip.sha256`

Beide Dateien nach Downloads speichern und prüfen:

```bash
cd ~/Downloads
sha256sum -c TeddyBench-Linux-1.7.0-linux.3.zip.sha256
```

Nur bei OK fortfahren:

```bash
TESTDIR=$(mktemp -d "$HOME/Downloads/teddybench-install.XXXXXX")
unzip TeddyBench-Linux-1.7.0-linux.3.zip -d "$TESTDIR"
cd "$TESTDIR/TeddyBench-Linux"
sudo bash ./install-linux.sh
```

Der Installer ersetzt die Programmdateien unter `/opt/teddybench` und erstellt `/usr/share/applications/teddybench.desktop`. Original-dog.ico und vorab erzeugtes PNG sind enthalten. Den Installer immer aus dem frisch entpackten Ordner starten, niemals aus `/opt/teddybench` selbst. Vor Updates eigene Konfigurationen sichern.

## Direkt im Terminal installieren

Der folgende vollständige Bash-Block lädt und prüft das ZIP, entpackt es temporär und startet erst danach den Installer. Bei einem Fehler wird abgebrochen. Die temporären Dateien werden anschließend entfernt. Voraussetzungen wie Mono werden nicht automatisch installiert.

```bash
(
  set -euo pipefail
  VERSION="1.7.0-linux.3"
  ARCHIVE="TeddyBench-Linux-${VERSION}.zip"
  BASE_URL="https://github.com/basecore/teddy/releases/download/v${VERSION}"
  INSTALL_TMP=$(mktemp -d)
  trap 'rm -rf "$INSTALL_TMP"' EXIT
  cd "$INSTALL_TMP"
  curl -fL --retry 3 -o "$ARCHIVE" "$BASE_URL/$ARCHIVE"
  curl -fL --retry 3 -o "$ARCHIVE.sha256" "$BASE_URL/$ARCHIVE.sha256"
  sha256sum -c "$ARCHIVE.sha256"
  unzip -q "$ARCHIVE"
  cd TeddyBench-Linux
  sudo bash ./install-linux.sh
)
```

### Dieselbe Installation als Einzeiler

In einem Bash-Terminal ausführen, nachdem TeddyBench geschlossen wurde:

```bash
(set -euo pipefail; VERSION=1.7.0-linux.3; ARCHIVE="TeddyBench-Linux-${VERSION}.zip"; BASE_URL="https://github.com/basecore/teddy/releases/download/v${VERSION}"; INSTALL_TMP=$(mktemp -d); trap 'rm -rf "$INSTALL_TMP"' EXIT; cd "$INSTALL_TMP"; curl -fL --retry 3 -o "$ARCHIVE" "$BASE_URL/$ARCHIVE"; curl -fL --retry 3 -o "$ARCHIVE.sha256" "$BASE_URL/$ARCHIVE.sha256"; sha256sum -c "$ARCHIVE.sha256"; unzip -q "$ARCHIVE"; cd TeddyBench-Linux; sudo bash ./install-linux.sh)
```

Nur vertrauenswürdige Releases installieren. Eine Prüfsumme aus demselben Release prüft Konsistenz, ist aber keine unabhängige digitale Signatur. Wer das Skript vorab prüfen möchte, verwendet die manuelle Installation. HTTP 404 bedeutet, dass Version oder Asset noch nicht verfügbar sind.

## Starten und entfernen

Im Mint-Menü nach TeddyBench suchen und bei Bedarf zu Favoriten hinzufügen. Die Anwendung als normaler Benutzer starten, nicht mit sudo. Alternativ:

```bash
/opt/teddybench/TeddyBench
```

Deinstallation:

```bash
sudo bash /opt/teddybench/uninstall-linux.sh
```

Das entfernt Programmordner und systemweiten Menüeintrag, nicht die separat gespeicherten CONTENT-Dateien.

## Releases, Testbuilds und Workflows

- `Build TeddyBench for Linux`: aktueller Quellcode-Build für Tests.
- `Release tested TeddyBench Linux build`: paketiert das erfolgreiche Artifact des festgelegten ac86aa2-Commits als linux.3; kein Neubuild und kein Überschreiben bestehender Releases.
- Andere ältere Release-/Packaging-Workflows sind nicht automatisch der aktuelle Veröffentlichungsweg.

Actions-Artifact-Downloads können ein äußeres ZIP um das eigentliche Paket legen. Direkt heruntergeladene Release-ZIPs enthalten nach einmaligem Entpacken den Programmordner. Für Tests immer Artifact und Commit prüfen; der Menüeintrag startet dagegen die installierte Version unter /opt.

## Roadmap: schrittweise verbessern

Die folgenden Punkte sind geplant beziehungsweise zu prüfen, nicht bereits implementiert und ohne zugesagten Termin:

1. Getestete Basis bewahren: Installer-Releases mit nachvollziehbarem Commit und Prüfsummen; Wiedergabe, Stoppen, Dialoge und Export vor jedem Release erneut testen.
2. Menüs und Oberfläche vereinheitlichen: Beschriftungen, Abstände, Buttonhöhen, Farben und Tastaturbedienung konsistent gestalten; Sortierung, Wiedergabe und Dateiaktionen sauber gruppieren. Vertraute TeddyBench-Struktur und funktionierende Textdarstellung erhalten.
3. Windows-spezifische Aufrufe erfassen und einzeln ersetzen: Dateimanager-/Browseraufrufe, Pfade, Laufwerkserkennung, Einstellungen, Aktualisierung und Audiopfade prüfen. Wo sinnvoll native Linux-Schnittstellen oder plattformübergreifende Bibliotheken einsetzen; Windows-Verhalten getrennt halten.
4. Audio weiter verbessern: Stoppen und Fehlerbehandlung absichern, Abhängigkeiten dokumentieren, Positionssuche und Zeitdarstellung gesondert entwickeln und testen. NAudio nur dort entfernen, wo Ersatz und Tests vorhanden sind.
5. Linux-Dateiablage verbessern: Einstellungen und Cache in benutzerspezifische Verzeichnisse verlagern; Schreibrechte und Updates robuster behandeln.
6. Leistung messen: Cover-/Metadaten-Caching, Hintergrundoperationen und große Bibliotheken untersuchen. Keine pauschalen Geschwindigkeitsversprechen ohne Messung.
7. Hardware separat validieren: NFC- und Firmwarefunktionen nur mit geeigneter Hardware und klaren Warnungen testen.

Ein einzelner Linux-kompatibler Funktionsersatz macht die gesamte GUI noch nicht nativ. Ein vollständiger Toolkit-Wechsel ist derzeit nicht Teil dieses Plans. Kleine, einzeln testbare Änderungen haben Vorrang vor einem erneuten kompletten GUI-Neubau.

## Fehler melden

Bitte Linux-/Mint-Version, Release beziehungsweise Commit, genaue Fehlermeldung, Startweg (Menü oder Testordner) und Schritte zur Reproduktion nennen. Logs vor dem Hochladen auf persönliche Informationen prüfen. Private CONTENT-Dateien nicht ungefragt veröffentlichen. Vor schreibenden oder Hardwareaktionen eigene Sicherungen anlegen.
