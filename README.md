# AeroSphere

AeroSphere is a .NET 8 WPF desktop dashboard with a Windows 10-style interface, local machine widgets, and a WiX-based installer pipeline.

## Current state

- Pure .NET 8 WPF application with a flat Windows 10-style shell.
- Working local-data components for time, system status, network adapters, drives, recent files, recent folders, and quick-launch actions.
- WiX v3 installer pipeline that publishes the app, harvests output with `heat`, and emits an MSI, a wrapped installer EXE, and a portable ZIP.
- Self-contained publish settings so the installed Start Menu shortcut launches the same output produced in CI.

## Project layout

```text
src/
  AeroSphere.App/
    App.xaml
    Infrastructure/
    Models/
    Services/
    Theming/
    ViewModels/
    MainWindow.xaml
installer/
  wix/
.github/
  workflows/
```

## Build locally

Prerequisites:

- Windows 10 version 19044 or newer, or Windows 11
- .NET 8 SDK
- Visual Studio 2022 17.8+ with desktop .NET tooling

Commands:

```powershell
dotnet restore src/AeroSphere.App/AeroSphere.App.csproj -p:Platform=x64
dotnet publish src/AeroSphere.App/AeroSphere.App.csproj `
  -c Release `
  -r win-x64 `
  --self-contained true `
  -p:Platform=x64 `
  -p:PublishSingleFile=false `
  -o artifacts/publish/win-x64
```

## Installer pipeline

The GitHub Actions workflow at [.github/workflows/build-installer.yml](/C:/aerosphere-main/.github/workflows/build-installer.yml) performs these steps:

1. Restores and publishes the WPF app for `win-x64`.
2. Harvests the published output into WiX source with `heat`.
3. Builds `AeroSphere.msi`.
4. Wraps the MSI in `AeroSphere-Installer.exe`.
5. Creates `AeroSphere-portable-win-x64.zip` from the publish output.
6. Uploads the installer and portable artifacts, plus a publish binlog on failure.

## Publish troubleshooting

The project is configured as a self-contained WPF desktop app. A few details matter for reliable CI publishing and launch behavior:

- The app is published self-contained for `win-x64`, so the installed Start Menu shortcut launches without requiring a separate desktop runtime install.
- The MSI creates a Start Menu shortcut for `AeroSphere.App.exe` inside the installed application folder.
- The workflow restores and publishes with `Platform=x64`, which keeps the installer output aligned with the target architecture.

If a publish step fails again, download the `aerosphere-build-logs-*` artifact from the failed run and inspect `publish.binlog` with MSBuild Structured Log Viewer.
