namespace AeroSphere.App.Models;

public sealed record OverviewCard(string Title, string Value, string Detail);

public sealed record RecentFileItem(
    string DisplayName,
    string LaunchPath,
    string SourceLabel,
    string LastModifiedLabel);

public sealed record RecentFolderItem(
    string DisplayName,
    string FullPath,
    string LastModifiedLabel,
    string ItemCountLabel);

public sealed record AppLauncher(
    string Name,
    string Description,
    string Target,
    string? Arguments = null);

public sealed record SystemFact(string Label, string Value);

public sealed record DriveSnapshot(
    string Name,
    string FreeSpaceLabel,
    string TotalSpaceLabel,
    string UsageLabel);

public sealed record NetworkSnapshot(
    string Name,
    string Status,
    string AddressText);

public sealed record DashboardSnapshot(
    string HeroTitle,
    string HeroSubtitle,
    IReadOnlyList<OverviewCard> OverviewCards,
    IReadOnlyList<SystemFact> SystemFacts,
    IReadOnlyList<DriveSnapshot> DriveSnapshots,
    IReadOnlyList<NetworkSnapshot> NetworkSnapshots);
