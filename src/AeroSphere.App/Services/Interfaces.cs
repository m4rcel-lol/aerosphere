using AeroSphere.App.Models;

namespace AeroSphere.App.Services;

public interface IWeatherService
{
    Task<WeatherSnapshot?> GetCurrentAsync(CancellationToken cancellationToken);
}

public interface IRecentItemsService
{
    Task<IReadOnlyList<RecentFileItem>> GetRecentFilesAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<RecentFolderItem>> GetRecentFoldersAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<RecentAppItem>> GetRecentAppsAsync(CancellationToken cancellationToken);
}

public interface IPrivacyService
{
    bool HasConsentForActivityHistory();
    bool IsPathExcluded(string path);
    string RedactPath(string path);
}
