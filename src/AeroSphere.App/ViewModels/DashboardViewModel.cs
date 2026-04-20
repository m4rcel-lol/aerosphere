using System.Collections.ObjectModel;
using AeroSphere.App.Models;

namespace AeroSphere.App.ViewModels;

public sealed class DashboardViewModel
{
    public ObservableCollection<WidgetDefinition> Widgets { get; } =
    [
        new("weather", "Weather Hub", WidgetSize.Large, true, 2, 1),
        new("planner", "Calendar / Planner", WidgetSize.Medium, true, 1, 1),
        new("insights", "Smart Daily Insights", WidgetSize.Medium, true, 1, 1),
        new("recent-files", "Recent Files", WidgetSize.Large, true, 1, 1),
        new("recent-folders", "Recent Folders", WidgetSize.Large, true, 1, 1),
        new("recent-apps", "Recent Apps", WidgetSize.Large, true, 1, 1),
        new("system", "System Overview", WidgetSize.Large, true, 1, 1),
        new("favorites", "Quick Access", WidgetSize.Large, true, 1, 1),
        new("utilities", "Productivity Utilities", WidgetSize.Large, true, 1, 1),
    ];
}
