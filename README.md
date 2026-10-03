<div align="center">

<img src="Shotora.App/Assets/Shotora.png" width="112" height="112" alt="Shotora logo" />

# Shotora

**Capture, annotate and read text from your screen, all on your own machine.**

A free, open-source screenshot tool that lives in your system tray. For Windows, macOS and Linux.

[![Latest release](https://img.shields.io/github/v/release/Adhamura/Shotora?label=release)](https://github.com/Adhamura/Shotora/releases/latest)
[![CI](https://github.com/Adhamura/Shotora/actions/workflows/ci.yml/badge.svg)](https://github.com/Adhamura/Shotora/actions/workflows/ci.yml)
[![License: Apache-2.0](https://img.shields.io/badge/license-Apache--2.0-blue)](LICENSE)

<a href="#download"><img src="https://img.shields.io/badge/Download%20Shotora-Windows%20%C2%B7%20macOS%20%C2%B7%20Linux-8B2BE2?style=for-the-badge&logo=github&logoColor=white" alt="Download Shotora for Windows, macOS and Linux" height="36" /></a>

[Download](#download) · [Features](#features) · [Platform support](#platform-support) · [Troubleshooting](#troubleshooting) · [Privacy](#privacy) · [Build from source](#build-from-source)

<br />

<img src="docs/images/hero.png" width="860" alt="Shotora capture overlay: a selected region annotated with a highlight, rectangles, an arrow and a text label, with the editor toolbar below" />

</div>

## Why Shotora

- **Private by design.** Screenshots and recognized text never leave your computer. No account, no cloud upload, no telemetry.
- **Annotate in place.** Draw a region and mark it up right on the screen: arrows, shapes, text, highlight, blur and pixelate.
- **Built-in OCR.** Release the mouse and the text inside your selection is recognized locally with Tesseract or EasyOCR.
- **Stays current.** Shotora checks GitHub Releases for new versions and installs them in place, only when you say so.

Shotora starts a capture with one hotkey or one click from the tray, then gets out of the way. It is built with
[Avalonia UI](https://avaloniaui.net/) on .NET 10, so it looks and behaves the same on every desktop it supports.

## Download

Get the latest version from the [Releases page](https://github.com/Adhamura/Shotora/releases/latest), or use the direct links below.
No .NET installation is needed: every build is self-contained.

| Platform | Installer | Portable |
| :-- | :-- | :-- |
| **Windows** x64 | [Shotora-win-x64-Setup.exe](https://github.com/Adhamura/Shotora/releases/latest/download/Shotora-win-x64-Setup.exe) | [Shotora-win-x64-Portable.zip](https://github.com/Adhamura/Shotora/releases/latest/download/Shotora-win-x64-Portable.zip) |
| **macOS** Apple Silicon | [Shotora-osx-arm64-Setup.pkg](https://github.com/Adhamura/Shotora/releases/latest/download/Shotora-osx-arm64-Setup.pkg) | [Shotora-osx-arm64-Portable.zip](https://github.com/Adhamura/Shotora/releases/latest/download/Shotora-osx-arm64-Portable.zip) |
| **macOS** Intel | [Shotora-osx-x64-Setup.pkg](https://github.com/Adhamura/Shotora/releases/latest/download/Shotora-osx-x64-Setup.pkg) | [Shotora-osx-x64-Portable.zip](https://github.com/Adhamura/Shotora/releases/latest/download/Shotora-osx-x64-Portable.zip) |
| **Linux** x64 | [Shotora-linux-x64.AppImage](https://github.com/Adhamura/Shotora/releases/latest/download/Shotora-linux-x64.AppImage) | The AppImage runs without installing |

The Windows installer sets Shotora up for the current user, so it needs no administrator rights.
Before you choose a build, check [Platform support](#platform-support): Linux needs an X11 session, and OCR is not
yet available in the Apple Silicon build.

> [!WARNING]
> **The builds are not code-signed yet**, so your OS warns you the first time you open one:
>
> - **Windows:** SmartScreen shows "Windows protected your PC". Click **More info**, then **Run anyway**.
> - **macOS:** Gatekeeper blocks the app or installer. Open **System Settings → Privacy & Security**, scroll down and click **Open Anyway**.
> - **Linux:** make the AppImage executable first with `chmod +x Shotora-linux-x64.AppImage`, then run it.
>
> To check that a download is intact, see [Verifying downloads](#verifying-downloads).

### Verifying downloads

GitHub shows a SHA-256 digest next to each asset on the [release page](https://github.com/Adhamura/Shotora/releases/latest).
Compute the hash of your download and compare the two values:

```powershell
# Windows (PowerShell)
Get-FileHash -Algorithm SHA256 .\Shotora-win-x64-Setup.exe
```

```bash
shasum -a 256 Shotora-osx-arm64-Setup.pkg     # macOS
sha256sum Shotora-linux-x64.AppImage          # Linux
```

## Platform support

| | Windows | macOS (Intel) | macOS (Apple Silicon) | Linux |
| :-- | :-: | :-: | :-: | :-: |
| Capture and annotation | Yes | Yes | Yes | X11 only ¹ |
| Global capture hotkeys | Yes | Not yet ² | Not yet ² | Not yet ² |
| OCR (Tesseract and EasyOCR) | Yes | Yes | Not yet ³ | Yes |
| Run on startup | Yes | Yes | Yes | Yes |
| In-app updates | Yes | Yes | Yes | Yes |

1. Wayland sessions are not supported for capture yet. Log in to an X11 (Xorg) session to use Shotora on Linux.
2. Global hotkeys are registered on Windows only for now. On macOS and Linux, start a capture from the tray menu.
3. The `osx-arm64` build does not bundle the native OCR runtimes yet (Tesseract libraries and the embedded Python used by
   EasyOCR). If you need OCR on an Apple Silicon Mac, install the Intel (`osx-x64`) build, which runs under Rosetta 2.

Windows builds target 64-bit Windows 10 and later. On Linux you also need a desktop with a system tray.

## Features

### Capture

- **Region capture.** Drag a rectangle anywhere on screen, then resize it with the handles or move it. Shotora remembers the last selection.
- **Full-screen capture.** Opens with the whole screen already selected.
- **Multi-monitor.** The capture overlay spans all connected displays.
- **Copy or save.** Copy to the clipboard (`Ctrl + C`, or right-click inside the selection), or save as PNG or JPEG (`Ctrl + S`).

### Annotation tools

<p align="center">
  <picture>
    <source media="(prefers-color-scheme: light)" srcset="docs/images/toolbar-light.png" />
    <img src="docs/images/toolbar-dark.png" width="720" alt="Shotora editor toolbar with the pointer, selection, undo, redo, move, drawing, blur, save, copy and close buttons" />
  </picture>
</p>

| Tool | What it does |
| :-- | :-- |
| **Pointer** | Select, drag and re-edit existing annotations. Double-click a text annotation to edit it. |
| **Select region** | Draw or redraw the capture selection. |
| **Move region** | Move the selection around the screen. |
| **Pen** | Freehand drawing. |
| **Line** / **Arrow** | Straight lines and arrows. |
| **Rectangle** / **Ellipse** | Outline shapes. |
| **Text** | Place text, with a choice of font, size, bold and italic. |
| **Highlight** | Wide, translucent freehand marker for emphasis. |
| **Blur** / **Pixelate** | Hide sensitive information before you share. |

The editor also has **Undo** and **Redo**, a **color palette** with a full color picker, and a **thickness** slider (1 to 12).
In **Settings → Editor** you can set the default color and thickness and hide any toolbar button you don't use.

### OCR (text recognition)

When you finish drawing a selection, Shotora recognizes the text inside it and shows the result in a small window with a
**Copy** button. Choose the engine (**Provider**) and recognition languages in **Settings → Main → OCR**.

| | Tesseract (default) | EasyOCR |
| :-- | :-- | :-- |
| **Runs** | Natively, inside Shotora | In an embedded Python runtime that ships with Shotora |
| **Setup** | None. Language data downloads the first time you use a language. | Click **Install EasyOCR** in **Settings → Main → OCR** |
| **Best for** | Clean screen text; fast and lightweight | Noisy or stylized text; deep-learning recognizer |
| **Disk use** | Small: one data file per language | Large: EasyOCR pulls in PyTorch and downloads its own models |

Both engines run entirely on your machine. Your screenshots are never uploaded.

### Customization

- **Three themes:** Dark, Light and Sunset, switchable without restarting.
- **73 interface languages.**
- **Configurable hotkeys** for capture and editor actions, plus configurable mouse buttons.
- **Saving:** PNG or JPEG (quality 50 to 100, default 90), a default folder (`Pictures/Shotora`), and a filename
  pattern with .NET date formatting, such as `Shotora_{0:yyyy-MM-dd_HH-mm-ss}`.
- **Run on startup** on Windows, macOS and Linux.

### Automatic updates

Shotora checks [GitHub Releases](https://github.com/Adhamura/Shotora/releases) shortly after it starts and then at most
once a day. When a new version is available, it shows the release notes and offers **Download and install**,
**Remind me later** or **Skip this version**. A prompt never interrupts a capture in progress: it waits until you close
the editor. To check manually, choose **Check for Updates** in the tray menu. To turn automatic checks off, clear
**Settings → Main → Automatically check for updates**.

Copies installed from a release update themselves in place and restart. A copy that cannot update itself, such as one
you built from source, links you to the release page instead.

## Usage

### Quick start

1. **Launch Shotora.** It starts in the system tray. Click the tray icon to open its menu: **Capture Region**, **Capture Full Screen**, **Settings**, **Check for Updates**, **About** and **Exit**.
2. **Capture.** Press the capture hotkey (Windows) or choose a capture from the tray menu, then drag a region.
3. **Annotate.** Pick a tool from the toolbar, and a color and thickness from the side panel.
4. **Finish.** Copy (`Ctrl + C` or right-click), save (`Ctrl + S`), or copy the recognized text from the OCR window. Press `Esc` to close.

### Default shortcuts

| Key or mouse | Action |
| :-- | :-- |
| `Ctrl + Shift + Print Screen` | Region capture (global, Windows) |
| `Alt + Print Screen` | Full-screen capture (global, Windows) |
| `Ctrl + Z` / `Ctrl + Y` | Undo / redo |
| `Ctrl + C` | Copy the selection to the clipboard and close |
| `Ctrl + S` | Save the selection to a file |
| Right-click inside the selection | Copy and close |
| Right-drag inside the selection | Move the selection |
| `Esc` | Return to the region tool, or close the editor |

Change any of these in **Settings → Hotkeys** and **Settings → Editor**.

## Screenshots

<table>
  <tr>
    <th align="center">Dark</th>
    <th align="center">Light</th>
    <th align="center">Sunset</th>
  </tr>
  <tr>
    <td><img src="docs/images/settings-dark.png" width="260" alt="Settings window, Dark theme" /></td>
    <td><img src="docs/images/settings-light.png" width="260" alt="Settings window, Light theme" /></td>
    <td><img src="docs/images/settings-sunset.png" width="260" alt="Settings window, Sunset theme" /></td>
  </tr>
</table>

<details>
<summary><strong>More screenshots:</strong> hotkeys, editor settings, updates and About</summary>
<br />

| | Dark | Light |
| :-- | :-- | :-- |
| **Hotkeys** | <img src="docs/images/settings-hotkeys-dark.png" width="300" alt="Hotkey settings, Dark theme" /> | <img src="docs/images/settings-hotkeys-light.png" width="300" alt="Hotkey settings, Light theme" /> |
| **Editor** | <img src="docs/images/settings-editor-dark.png" width="300" alt="Editor settings, Dark theme" /> | <img src="docs/images/settings-editor-light.png" width="300" alt="Editor settings, Light theme" /> |
| **Update** | <img src="docs/images/update-dark.png" width="300" alt="Software Update window, Dark theme" /> | <img src="docs/images/update-light.png" width="300" alt="Software Update window, Light theme" /> |
| **About** | <img src="docs/images/about-dark.png" width="220" alt="About window, Dark theme" /> | |

</details>

## Troubleshooting

<details>
<summary><strong>Linux: "Screen capture is not supported under Wayland"</strong></summary>
<br />

Shotora captures the screen through X11. Log out, pick an Xorg or X11 session on your login screen (often in a gear menu),
and log back in. Wayland support is not available yet.

</details>

<details>
<summary><strong>macOS: captures are blank or show only the desktop wallpaper</strong></summary>
<br />

macOS needs **Screen Recording** permission for Shotora. Open **System Settings → Privacy & Security → Screen Recording**
(called **Screen & System Audio Recording** on newer macOS), turn Shotora on, then choose **Exit** from the tray menu and start Shotora again.

</details>

<details>
<summary><strong>Windows: a capture hotkey does nothing</strong></summary>
<br />

Another app may already own that key combination. Windows lets only one app register a global hotkey, and `Print Screen`
combinations are popular with other screenshot and cloud-storage tools. Pick a different shortcut in **Settings → Hotkeys**,
or start the capture from the tray menu.

</details>

<details>
<summary><strong>OCR returns no text</strong></summary>
<br />

Check that the right recognition language is set in **Settings → Main → OCR**. The first time you use a Tesseract language,
Shotora downloads its data file, so that first recognition needs an internet connection. On Apple Silicon Macs, OCR needs
the Intel build for now (see [Platform support](#platform-support)).

</details>

## Privacy

Shotora has no telemetry, no analytics and no account. Captures, annotations and recognized text stay on your computer.
Shotora goes online only in these cases:

| When | Where | Why |
| :-- | :-- | :-- |
| Automatic or manual update check | `api.github.com` and `github.com/Adhamura/Shotora/releases` | Find out whether a newer version exists, and download it when you choose to |
| You use a Tesseract language for the first time | `github.com/tesseract-ocr/tessdata_best` (fallback `tessdata_fast`) | Download that language's `.traineddata` file |
| You click **Install EasyOCR** | `bootstrap.pypa.io` and the Python Package Index | Install `pip` and the `easyocr` package into Shotora's embedded Python |
| First EasyOCR recognition for a language | EasyOCR's model host | The EasyOCR library downloads its detection and recognition models |

Settings are stored in `Shotora/config.json` under your user application-data folder (`%APPDATA%` on Windows, `~/.config`
on macOS and Linux), next to downloaded Tesseract language files (`Shotora/tessdata`).

## Build from source

**Prerequisites:** [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) and Git. No separate Python installation is needed.

```bash
git clone https://github.com/Adhamura/Shotora.git
cd Shotora
dotnet tool restore                      # Cake and the Velopack CLI (vpk), pinned in dotnet-tools.json
dotnet build Shotora.sln
dotnet run --project Shotora.App/Shotora.App.csproj
```

Run the tests the same way CI does. The Avalonia UI tests share one dispatcher thread, so test collections run serially:

```bash
dotnet test Shotora.App.Tests -c Release -- xUnit.ParallelizeTestCollections=false
```

<details>
<summary><strong>Publishing and packaging</strong> (Cake and Velopack)</summary>
<br />

Self-contained builds are produced by the Cake script [`build.cake`](build.cake). Always pass `--runtime`, because the script's
default runtime is `linux-arm64`:

```bash
dotnet tool run dotnet-cake --target=Publish --runtime=win-x64 --configuration=Release \
  --selfcontained=true --appversion=1.2.3 --output=./publish/win-x64
```

| Argument | Purpose |
| :-- | :-- |
| `--runtime` | Target RID. Released platforms are `win-x64`, `osx-arm64`, `osx-x64` and `linux-x64`. |
| `--configuration` | `Release` (default) or `Debug` |
| `--selfcontained` / `--singlefile` | Bundle the .NET runtime / publish as a single file |
| `--appversion` | Stamps the assemblies with a version. The in-app updater compares against it. |
| `--output` | Publish directory (default `./ready`) |

The release pipeline then packages each platform with the Velopack CLI:

```bash
dotnet tool run vpk pack --packId Shotora --packTitle Shotora --packVersion 1.2.3 \
  --packDir ./publish/win-x64 --mainExe Shotora.App.exe --runtime win-x64 --channel win-x64 \
  --icon Shotora.App/Assets/Shotora.ico --outputDir ./releases
```

`vpk` targets an older .NET runtime, so set `DOTNET_ROLL_FORWARD=LatestMajor` when you run it on a machine that only has .NET 10.
Each platform must be packed on its own OS (`Setup.exe` on Windows, `.pkg` on macOS, `.AppImage` on Linux). See
[`.github/workflows/release.yml`](.github/workflows/release.yml) for the exact steps.

</details>

### Architecture

| Project | Responsibility |
| :-- | :-- |
| `Shotora.App` | Avalonia application: windows, custom controls, styles and themes, 73 localization dictionaries, startup and DI composition |
| `Shotora.App.Interfaces` | Service, provider and view-model contracts |
| `Shotora.App.Models` | Settings, enums, constants, drawing and view-model base models |
| `Shotora.App.Services` | Implementations: capture, drawing, OCR (Tesseract and EasyOCR), hotkeys, tray, settings and updates |
| `Shared.Interfaces` / `Shared.Models` / `Shared.Services` | Cross-cutting adapters and facades (file system, environment, HTTP) shared by all layers |
| `NativeSupport` | Bundled native Tesseract and Leptonica libraries and the loader that resolves them per OS and architecture |
| `Shotora.App.Tests` | xUnit tests with Moq and Avalonia.Headless |

The app follows **MVVM** with [CommunityToolkit.Mvvm](https://github.com/CommunityToolkit/dotnet). Views live in `Shotora.App`,
contracts in `*.Interfaces`, and implementations in `*.Services`. Services are registered **by convention**: every class
`Foo` that implements `IFoo` is added to `Microsoft.Extensions.DependencyInjection` automatically. UI tokens, theme brushes
and component rules are documented in the [design system](docs/design/DESIGN_SYSTEM.md).

### Releasing

Releases are fully automated by [`.github/workflows/release.yml`](.github/workflows/release.yml):

1. Push a tag such as `v1.2.0`, or run the **Release** workflow manually and enter a version.
2. The workflow builds and tests the solution, then publishes and packs `win-x64`, `linux-x64`, `osx-arm64` and `osx-x64`
   on their native runners. It generates release notes from the commit log.
3. It creates a single GitHub release with every installer, portable package and Velopack feed file. Installed copies of
   Shotora pick it up through the in-app updater. A version with a suffix (for example `1.2.0-beta.1`) is published as a pre-release.

## Contributing

Issues and pull requests are welcome.

- **Get help, report a bug or request a feature** on the [issue tracker](https://github.com/Adhamura/Shotora/issues). Include your OS, the Shotora version (shown in **About**) and steps to reproduce.
- **Before opening a PR**, make sure `dotnet build` and the test command above pass. [CI](.github/workflows/ci.yml) runs both on every pull request.
- **UI changes** should follow the [design system](docs/design/DESIGN_SYSTEM.md): use the shared tokens and control styles,
  and add every new brush to all three themes.
- **Translations** live in `Shotora.App/Resources/Languages/Strings.<code>.axaml`. A new string needs an entry in every language file.

## License

Shotora is licensed under the [Apache License 2.0](LICENSE).

## Acknowledgements

Shotora builds on these open-source projects:
[Avalonia UI](https://github.com/AvaloniaUI/Avalonia) ·
[Velopack](https://github.com/velopack/velopack) ·
[SkiaSharp](https://github.com/mono/SkiaSharp) ·
[Tesseract](https://github.com/tesseract-ocr/tesseract) (via the [Tesseract .NET wrapper](https://github.com/charlesw/tesseract)) ·
[EasyOCR](https://github.com/JaidedAI/EasyOCR) ·
[CommunityToolkit.Mvvm](https://github.com/CommunityToolkit/dotnet) ·
[H.NotifyIcon](https://github.com/HavenDV/H.NotifyIcon) ·
[Cake](https://github.com/cake-build/cake) ·
[xUnit](https://github.com/xunit/xunit) and [Moq](https://github.com/devlooped/moq).

## Author

**Kazuki Kimura**:
[Website](https://www.kazukikimura.com) ·
[LinkedIn](https://www.linkedin.com/in/kazuki-kimura/) ·
[Email](mailto:kazuki.kimura.jp@gmail.com) ·
[LINE](https://line.me/ti/p/adhamura) ·
[Instagram](https://www.instagram.com/kimura_kazuki_official/)

<sub>If Shotora is useful to you, a star on GitHub helps other people find it.</sub>
