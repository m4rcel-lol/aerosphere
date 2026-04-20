# AeroSphere — Premium Windows Personal Dashboard

## 1) Product summary
AeroSphere is a native-feeling Windows 10/11 desktop dashboard built for daily productivity. It combines weather, recent activity (files/folders/apps), system health, calendar context, favorites, and smart insights in one customizable home screen.

Core outcomes:
- **Daily utility**: gives users high-value information and actions within 2–3 clicks.
- **Premium UX**: Fluent-inspired visuals, careful spacing, and smooth interactions.
- **Privacy-first**: local history processing by default, opt-in data sharing only.
- **Reliable on Win10/11**: layered design/fallback strategy for visual and API differences.

---

## 2) Design vision
**Design intent**: “A calm command center.”

AeroSphere should feel like a modern Windows companion—not a browser dashboard wrapped in a desktop shell.

- **Windows 11**: Mica-like shell surfaces, rounded 12–16px cards, soft depth, restrained translucency.
- **Windows 10**: equivalent hierarchy using acrylic-inspired/solid surfaces with preserved spacing, contrast, and component geometry.
- **Density profiles**: Compact / Comfortable / Spacious to fit different display sizes and user preference.
- **Motion language**: subtle entrance, hover elevation, and state transitions under 180ms by default.

---

## 3) Information architecture
Top-level nav:
1. Dashboard
2. Activity
3. Files
4. Apps
5. System
6. Shortcuts
7. Settings

Cross-cutting entities:
- `RecentFileItem`
- `RecentFolderItem`
- `RecentAppItem`
- `PinnedItem`
- `WidgetDefinition`
- `WidgetLayout`
- `Insight`
- `SystemSnapshot`
- `WeatherSnapshot`

Primary user flows:
- First launch setup (theme + privacy + weather location)
- Daily glance (Dashboard)
- Fast retrieval (Search/Command Palette)
- Context action (right-click on item)
- Personalization (Edit layout mode)

---

## 4) Full dashboard layout description
### Shell regions
1. **Left navigation rail** (collapsible)
   - App logo/title
   - Primary destinations
   - Edit layout button
   - Bottom zone: Settings + About

2. **Top bar**
   - Unified search box (Ctrl+K)
   - Weather micro-summary (temp + condition icon)
   - Date/time block (click opens calendar overlay)
   - Refresh action
   - Quick Add / Quick Launch
   - Profile/Settings menu

3. **Main responsive grid**
   - 12-column layout desktop widescreen
   - 8-column on mid-width
   - 4-column compact

### Proposed default card arrangement (balanced hierarchy)
- Row 1:
  - Weather Hub (Large, 6 cols)
  - Today Planner (Medium, 3 cols)
  - Smart Daily Insights (Medium, 3 cols)
- Row 2:
  - Recent Files (Large, 4 cols)
  - Recent Folders (Large, 4 cols)
  - Recent Apps (Large, 4 cols)
- Row 3:
  - System Overview (Large, 4 cols)
  - Quick Access/Favorites (Large, 4 cols)
  - Productivity Utilities stack (Large, 4 cols)

4. **Quick actions strip** (optional footer)
   - Open Downloads
   - Open Documents
   - File Explorer
   - Task Manager
   - Restart / Shutdown (with confirmation)
   - Custom shortcuts

---

## 5) Widget-by-widget functional specification
### 5.1 Weather Hub
Features:
- Current: temp, condition, high/low, feels-like, humidity, wind, rain chance
- Hourly forecast (next 12–24h)
- Daily forecast (5–7 day)
- Auto-location with manual city fallback
- Cached latest successful forecast for offline mode

States:
- Loading skeleton
- Offline cached (timestamp shown)
- No internet + no cache (empty instructional state)
- API error (retry affordance)

### 5.2 Recent Folders
Data strategy:
- `Recent Items` shell links
- Parent folders derived from recent files
- Explorer MRU sources (when available via supported APIs)

Per row:
- Name, full path (with truncation tooltip), last opened, pin icon
- Context menu: Open, Open in Explorer, Copy Path, Pin/Unpin, Exclude

Fallback:
- If unavailable due policy/privacy: clear empty state with action to enable tracking.

### 5.3 Recent Files
Per row:
- file icon, name, type, parent path, app association, last opened
- actions: Open, Open containing folder, Copy path, Pin, Remove from recents view

Filtering:
- Docs, Images, Video, Code, Spreadsheet, PDF, Archive, Other
- Search-in-widget

Error handling:
- Missing/moved file shown as unavailable with muted style and cleanup action.

### 5.4 Recent Apps
Signals (best effort):
- AppUserModelID / Start menu launches
- Jump list recency where accessible
- Optional usage telemetry (local only, opt-in)

Capabilities:
- Launch app
- Pin to favorites
- Sort: Recent / Most used / Pinned / Category

### 5.5 System Overview
Metrics:
- CPU, RAM, Disk, Network, Battery (if present), Uptime, optional GPU
Visualization:
- mini rings/bars + sparkline trends for last 60 minutes

States:
- Desktop w/o battery -> graceful “No battery detected.”

### 5.6 Quick Access / Favorites
Supports mixed item types:
- Apps, files, folders, URLs, commands

Features:
- Drag-and-drop reorder
- Edit labels/icons
- Categories (Work, Personal, Tools)

### 5.7 Calendar / Day Planner
- Today header + weekday
- Upcoming events (if connected calendar provider/local source)
- Focus block for top 3 tasks
- Month mini-view toggle

### 5.8 Productivity Utilities
Enabled by user:
- Notes scratchpad
- To-do list
- Downloads monitor
- Recent screenshots
- Focus timer
- Media controls
- Clipboard summary (privacy-aware)

### 5.9 Smart Daily Insights
Rule-based non-spam panel surfaces at most 3–5 items/day:
- Weather alert
- Low disk warning
- Battery health concern
- Frequently reopened folder suggestion
- Unfinished recent files reminder

Controls:
- Dismiss / Snooze / Disable insight type

---

## 6) Visual design system
Typography:
- `Segoe UI Variable` primary
- fallback `Segoe UI`

Tokens:
- Corner radii: 8 / 12 / 16
- Spacing scale: 4, 8, 12, 16, 24, 32
- Elevation: 0, 1, 2
- Motion: 80ms micro, 160ms standard, 220ms complex

Color strategy:
- Theme-aware semantic tokens (`Surface`, `SurfaceAlt`, `TextPrimary`, `TextSecondary`, `Accent`)
- High-contrast overrides
- Avoid low-opacity text over transparent surfaces

Iconography:
- Fluent System Icons style, consistent stroke weight

---

## 7) Data source and technical implementation strategy
Preferred stack:
- **C# + .NET 8 + WinUI 3 + Windows App SDK + MVVM Toolkit**

Key services:
- `IWeatherService`: fetch + cache + offline fallback
- `IRecentItemsService`: shell recents + safe resolvers
- `IAppUsageService`: recent app signals (best effort, explicit transparency)
- `ISystemMetricsService`: performance counters/WMI/Power APIs
- `ISettingsService`: JSON settings + schema versioning
- `IPrivacyService`: consent gates + exclusions + redact mode
- `ISearchService`: indexed in-memory aggregate for fast command palette

Persistence:
- Local app data (`%LocalAppData%\AeroSphere`)
- SQLite for history indexes + JSON for preferences

Resilience:
- Layered providers with scoring and dedupe
- Per-widget circuit breakers and stale-cache fallback

---

## 8) Privacy and security model
Defaults:
- Activity tracking OFF by default until user consents
- Local storage only by default
- No admin rights required for standard operation

Controls:
- Clear all history
- Per-source toggles (Files/Folders/Apps)
- Exclusion lists for paths, extensions, and app executables
- Path redaction mode (e.g., `C:\Users\***\Documents`)

Security:
- Validate paths before launching
- Safe process start rules
- No hidden network uploads of personal history

---

## 9) Accessibility strategy
- Full keyboard traversal + predictable tab order
- Visible focus ring on all interactive controls
- Narrator labels + AutomationProperties on cards/actions
- Supports 100–200% scaling
- High contrast mode token overrides
- Reduced motion setting disables non-essential animations
- Text never dependent on color alone

---

## 10) Performance strategy
- Cold start target: < 1.8s on mid-tier hardware
- Idle CPU target: < 1%
- Async widget loading with prioritized above-the-fold content
- Debounced refreshes and background timers with cancellation
- Shared memory cache for icon extraction and recents normalization
- Lazy-load heavy widgets and charts

---

## 11) Recommended project structure / folder structure
```text
src/
  AeroSphere.App/
    App.xaml
    App.xaml.cs
    MainWindow.xaml
    MainWindow.xaml.cs
    Views/
      DashboardPage.xaml
      SettingsPage.xaml
      Widgets/
        WeatherWidget.xaml
        RecentFilesWidget.xaml
        RecentFoldersWidget.xaml
        RecentAppsWidget.xaml
        SystemWidget.xaml
        PlannerWidget.xaml
        InsightsWidget.xaml
    ViewModels/
      ShellViewModel.cs
      DashboardViewModel.cs
      SettingsViewModel.cs
      Widgets/
    Models/
      Recents/
      Weather/
      System/
      Insights/
    Services/
      Interfaces/
      Implementations/
      Providers/
    Theming/
      ThemeTokens.xaml
      ThemeManager.cs
    Infrastructure/
      Storage/
      Logging/
      Diagnostics/
```

---

## 12) Full implementation plan
### Phase 1 — Foundation (Week 1-2)
- Bootstrap WinUI 3 shell + MVVM
- Theme system (light/dark/system + accent)
- Widget host with resize/reorder plumbing
- Settings persistence and schema versioning

### Phase 2 — Core data widgets (Week 3-4)
- Weather (provider + cache)
- Recent files/folders/apps providers
- System metrics card
- Unified search index first pass

### Phase 3 — Personalization + privacy (Week 5)
- Consent dialogs and privacy dashboard
- Exclusions and history clear workflows
- Pinning and quick access editor

### Phase 4 — Quality + accessibility (Week 6)
- Narrator/keyboard/high-contrast polish
- Perf profiling and startup optimization
- Error-state and empty-state refinement

### Phase 5 — Stabilization and release (Week 7)
- Automated tests + telemetry guardrails
- Installer packaging and update strategy

---

## 13) Code architecture and major classes/components
- `ShellViewModel`: nav state, top bar commands, global refresh
- `DashboardViewModel`: card layout + widget orchestration
- `WidgetHostControl`: drag/resize + persistence hooks
- `WeatherWidgetViewModel`: fetch policy and cache timestamps
- `RecentFilesWidgetViewModel`, `RecentFoldersWidgetViewModel`, `RecentAppsWidgetViewModel`
- `SystemOverviewViewModel`: rolling metric buffers
- `InsightsEngine`: rule evaluation over local signals
- `CommandPaletteService`: cross-domain search and actions
- `PrivacyGateService`: feature-level consent and redaction

---

## 14) Sample UI structure or full source code
See `src/AeroSphere.App` for a production-minded starter shell with:
- WinUI app entry
- Navigation + top bar
- Dashboard card grid scaffolding
- Theme token dictionaries
- Widget model contracts

---

## 15) Styling/theme guidance
- Use Windows accent by default with user override palette.
- Apply blur/translucency only at high-level surfaces; keep cards mostly opaque for readability.
- Maintain minimum text contrast ratio of 4.5:1.
- Use subtle gradients on hero cards (Weather, Insights), avoid noisy backgrounds.

---

## 16) Setup and run instructions
Prerequisites:
- Windows 10 (19044+) or Windows 11
- .NET 8 SDK
- Visual Studio 2022 (17.8+) with WinUI/Desktop workload

Steps:
1. Open `src/AeroSphere.App` in Visual Studio.
2. Restore NuGet packages.
3. Build and run Debug x64.
4. On first launch, complete Privacy + Weather onboarding.

---


### CI/CD installer workflow (WiX)
- GitHub Actions workflow: `.github/workflows/build-installer.yml`
- Publishes WinUI app for `win-x64` using self-contained release output
- Uses **WiX v3** tooling (`heat`, `candle`, `light`) to:
  1. harvest published files into a component group
  2. build `AeroSphere.msi`
  3. wrap MSI in `AeroSphere-Setup.exe` bootstrapper
- Uploads both MSI and EXE as CI artifacts for each run

## 17) Testing checklist
Functional:
- Weather online/offline/cache behavior
- Recents list population and context menu actions
- Missing file handling
- App launching and pinning
- Widget drag/resize persistence

Accessibility:
- Keyboard-only navigation
- Narrator readouts
- 100–200% DPI scaling
- High contrast validation

Performance:
- Startup and idle CPU budget
- Refresh stress test
- Memory growth over 8-hour run

Privacy:
- Consent gating
- Clear history
- Exclusion rules
- Redaction mode verification

---

## 18) Future enhancement ideas
- Optional Microsoft 365 / Google Calendar connector
- AI-assisted “focus for today” summaries (fully opt-in)
- Plugin SDK for third-party widgets
- Cross-device layout sync (opt-in, encrypted)
- Touch-friendly tablet mode profile
