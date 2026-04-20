namespace AeroSphere.App.Models;

public enum WidgetSize
{
    Small,
    Medium,
    Large,
}

public sealed record WidgetDefinition(
    string Id,
    string Title,
    WidgetSize Size,
    bool IsEnabled,
    int ColumnSpan,
    int RowSpan);

public sealed record RecentFileItem(
    string FileName,
    string FullPath,
    string? AssociatedApp,
    DateTimeOffset LastOpened,
    bool IsMissing);

public sealed record RecentFolderItem(
    string DisplayName,
    string FullPath,
    DateTimeOffset LastOpened);

public sealed record RecentAppItem(
    string AppName,
    string LaunchCommand,
    DateTimeOffset LastOpened,
    int LaunchCount);

public sealed record WeatherSnapshot(
    string Location,
    decimal TemperatureC,
    string Condition,
    decimal HighC,
    decimal LowC,
    int HumidityPercent,
    decimal WindKph,
    DateTimeOffset RetrievedAt,
    bool IsFromCache);
