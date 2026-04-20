using AeroSphere.App.Models;

namespace AeroSphere.App.Services;

public interface ISystemSnapshotService
{
    DashboardSnapshot GetSnapshot();
}

public interface IRecentContentService
{
    IReadOnlyList<RecentFileItem> GetRecentFiles(int maxItems);
    IReadOnlyList<RecentFolderItem> GetRecentFolders(int maxItems);
}

public interface ILauncherService
{
    IReadOnlyList<AppLauncher> GetLaunchers();
    void Launch(AppLauncher launcher);
    void OpenPath(string path);
}
