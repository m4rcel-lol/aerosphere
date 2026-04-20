# AeroSphere

AeroSphere is a WinUI 3 desktop dashboard shell for Windows 10/11. The project focuses on a clean command-center layout for weather, recent activity, quick actions, and system context, with WiX-based installer output in CI.

## Current state

- Native WinUI 3 shell with a navigation rail, search/header bar, hero summary panel, and data-driven dashboard cards.
- Core domain models for widgets, recent items, and weather snapshots.
- WiX v3 installer pipeline that publishes the app, harvests output with `heat`, and emits both `AeroSphere.msi` and `AeroSphere-Setup.exe`.
- Unpackaged, self-contained publish settings so the CI build targets the same deployment shape used by the installer.

## Project layout

```text
src/
  AeroSphere.App/
    App.xaml
    MainWindow.xaml
    Models/
    Services/
    Theming/
    ViewModels/
installer/
  wix/
.github/
  workflows/
```

## Build locally

Prerequisites:

- Windows 10 version 19044 or newer, or Windows 11
- .NET 8 SDK
- Visual Studio 2022 17.8+ with the WinUI / Windows App SDK desktop tooling

Commands:

```powershell
dotnet restore src/AeroSphere.App/AeroSphere.App.csproj -p:Platform=x64
dotnet publish src/AeroSphere.App/AeroSphere.App.csproj `
  -c Release `
  -r win-x64 `
  --self-contained true `
  -p:Platform=x64 `
  -p:WindowsPackageType=None `
  -p:AppxPackage=false `
  -p:PublishSingleFile=false `
  -o artifacts/publish/win-x64
```

## Installer pipeline

The GitHub Actions workflow at [.github/workflows/build-installer.yml](/C:/aerosphere-main/.github/workflows/build-installer.yml) performs these steps:

1. Restores and publishes the WinUI app for `win-x64`.
2. Harvests the published output into WiX source with `heat`.
3. Builds `AeroSphere.msi`.
4. Wraps the MSI in `AeroSphere-Setup.exe`.
5. Uploads the installer artifacts, plus a publish binlog on failure.

## Publish troubleshooting

The project is configured as an unpackaged WinUI desktop app. Two details matter for reliable CI publishing:

- The project file sets `WindowsPackageType=None`, `AppxPackage=false`, and `WindowsAppSDKSelfContained=true`.
- The workflow restores and publishes with `Platform=x64`, which helps the WinUI XAML compiler stay aligned with the `win-x64` runtime identifier.

If `XamlCompiler.exe` fails again, download the `aerosphere-build-logs-*` artifact from the failed run and inspect `publish.binlog` with MSBuild Structured Log Viewer.
