using System.Collections.ObjectModel;
using AeroSphere.App.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AeroSphere.App.ViewModels;

public sealed partial class DashboardViewModel : ObservableObject
{
    public DashboardViewModel()
    {
        Refresh();
    }

    [ObservableProperty]
    private string headerTitle = "A calm command center for the desktop";

    [ObservableProperty]
    private string headerSubtitle = "Weather, recents, system context, and quick actions stay in one responsive WinUI shell that is ready for real data providers.";

    [ObservableProperty]
    private string focusSummary = "Focus: tighten the shell, surface the right context, and keep everything one or two clicks away.";

    [ObservableProperty]
    private string weatherSummary = "Warsaw | 71 F | Clear";

    [ObservableProperty]
    private string timeSummary = string.Empty;

    [ObservableProperty]
    private string statusSummary = "Starter shell ready";

    [ObservableProperty]
    private string lastUpdated = string.Empty;

    public ObservableCollection<WidgetDefinition> Widgets { get; } =
    [
        new(
            "weather",
            "Weather Hub",
            "Ambient context",
            "Current conditions, short-range forecast, and offline cache status in one glanceable card.",
            "71 F",
            "7-day outlook primed",
            WidgetSize.Large,
            true),
        new(
            "planner",
            "Calendar / Planner",
            "Today view",
            "Pin the next meeting, focus block, and reminders without burying the current task.",
            "3 key events",
            "Top priority: design review",
            WidgetSize.Medium,
            true),
        new(
            "insights",
            "Smart Daily Insights",
            "Signals",
            "Highlight the few warnings or nudges that matter instead of turning the dashboard into noise.",
            "2 new insights",
            "Disk and weather checks ready",
            WidgetSize.Medium,
            true),
        new(
            "recent-files",
            "Recent Files",
            "Activity",
            "Bring documents, code, screenshots, and downloads back with quick path and app context.",
            "24 items indexed",
            "Last opened 8 minutes ago",
            WidgetSize.Large,
            true),
        new(
            "recent-folders",
            "Recent Folders",
            "Navigation",
            "Promote the folders that keep showing up so users can jump back without hunting in Explorer.",
            "12 folders tracked",
            "Pinned and filtered views planned",
            WidgetSize.Large,
            true),
        new(
            "recent-apps",
            "Recent Apps",
            "Launch",
            "Show the apps that were actually used recently, not just whatever happens to be installed.",
            "8 active apps",
            "Sort by recency or usage",
            WidgetSize.Large,
            true),
        new(
            "system",
            "System Overview",
            "Health",
            "Keep CPU, memory, battery, and network trends close enough to act before they become friction.",
            "Idle target < 1%",
            "Battery-aware on supported hardware",
            WidgetSize.Large,
            true),
        new(
            "favorites",
            "Quick Access",
            "Shortcuts",
            "Mix apps, files, folders, and commands into one customizable launch strip for daily routines.",
            "Work + personal",
            "Drag reorder planned",
            WidgetSize.Large,
            true),
        new(
            "utilities",
            "Productivity Utilities",
            "Tools",
            "Keep small helpers such as notes, timers, and download watchlists one tap from the shell.",
            "Notes, timer, media",
            "Privacy-aware by default",
            WidgetSize.Large,
            true),
    ];

    [RelayCommand]
    private void Refresh()
    {
        var now = DateTimeOffset.Now;

        TimeSummary = now.ToString("ddd, MMM d | h:mm tt");
        LastUpdated = now.ToString("MMM d, yyyy | h:mm tt");
        StatusSummary = "Dashboard snapshot refreshed";
    }
}
