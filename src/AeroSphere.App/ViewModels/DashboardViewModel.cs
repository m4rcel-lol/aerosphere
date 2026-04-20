using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using AeroSphere.App.Infrastructure;
using AeroSphere.App.Models;
using AeroSphere.App.Services;

namespace AeroSphere.App.ViewModels;

public sealed class DashboardViewModel : ObservableObject, IDisposable
{
    private readonly ISystemSnapshotService _systemSnapshotService;
    private readonly IRecentContentService _recentContentService;
    private readonly ILauncherService _launcherService;
    private readonly DispatcherTimer _clockTimer;

    private string _heroTitle = "Windows 10 style desktop dashboard";
    private string _heroSubtitle = "A faster, flatter local dashboard that works with standard .NET deployment and a normal Windows installer.";
    private string _currentTime = string.Empty;
    private string _currentDate = string.Empty;
    private string _machineName = Environment.MachineName;
    private string _statusMessage = "Loading dashboard...";
    private string _lastRefreshLabel = "Not refreshed yet";

    public DashboardViewModel()
        : this(new SystemSnapshotService(), new RecentContentService(), new LauncherService())
    {
    }

    internal DashboardViewModel(
        ISystemSnapshotService systemSnapshotService,
        IRecentContentService recentContentService,
        ILauncherService launcherService)
    {
        _systemSnapshotService = systemSnapshotService;
        _recentContentService = recentContentService;
        _launcherService = launcherService;

        RefreshCommand = new RelayCommand(async _ => await RefreshAsync());
        LaunchAppCommand = new RelayCommand(parameter => Launch(parameter as AppLauncher), parameter => parameter is AppLauncher);
        OpenRecentFileCommand = new RelayCommand(parameter => OpenRecentFile(parameter as RecentFileItem), parameter => parameter is RecentFileItem);
        OpenRecentFolderCommand = new RelayCommand(parameter => OpenRecentFolder(parameter as RecentFolderItem), parameter => parameter is RecentFolderItem);

        _clockTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1),
        };
        _clockTimer.Tick += (_, _) => UpdateClock();
        UpdateClock();
        _clockTimer.Start();

        _ = RefreshAsync();
    }

    public string HeroTitle
    {
        get => _heroTitle;
        set => SetProperty(ref _heroTitle, value);
    }

    public string HeroSubtitle
    {
        get => _heroSubtitle;
        set => SetProperty(ref _heroSubtitle, value);
    }

    public string CurrentTime
    {
        get => _currentTime;
        set => SetProperty(ref _currentTime, value);
    }

    public string CurrentDate
    {
        get => _currentDate;
        set => SetProperty(ref _currentDate, value);
    }

    public string MachineName
    {
        get => _machineName;
        set => SetProperty(ref _machineName, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public string LastRefreshLabel
    {
        get => _lastRefreshLabel;
        set => SetProperty(ref _lastRefreshLabel, value);
    }

    public ObservableCollection<OverviewCard> OverviewCards { get; } = [];

    public ObservableCollection<RecentFileItem> RecentFiles { get; } = [];

    public ObservableCollection<RecentFolderItem> RecentFolders { get; } = [];

    public ObservableCollection<AppLauncher> AppLaunchers { get; } = [];

    public ObservableCollection<AppLauncher> PinnedLaunchers { get; } = [];

    public ObservableCollection<SystemFact> SystemFacts { get; } = [];

    public ObservableCollection<DriveSnapshot> DriveSnapshots { get; } = [];

    public ObservableCollection<NetworkSnapshot> NetworkSnapshots { get; } = [];

    public ICommand RefreshCommand { get; }

    public ICommand LaunchAppCommand { get; }

    public ICommand OpenRecentFileCommand { get; }

    public ICommand OpenRecentFolderCommand { get; }

    public void Dispose()
    {
        _clockTimer.Stop();
    }

    private async Task RefreshAsync()
    {
        try
        {
            StatusMessage = "Refreshing local data...";

            var dashboardTask = Task.Run(_systemSnapshotService.GetSnapshot);
            var recentFilesTask = Task.Run(() => _recentContentService.GetRecentFiles(14));
            var recentFoldersTask = Task.Run(() => _recentContentService.GetRecentFolders(12));
            var launchersTask = Task.Run(_launcherService.GetLaunchers);

            await Task.WhenAll(dashboardTask, recentFilesTask, recentFoldersTask, launchersTask);

            var snapshot = dashboardTask.Result;
            HeroTitle = snapshot.HeroTitle;
            HeroSubtitle = snapshot.HeroSubtitle;
            MachineName = snapshot.SystemFacts.FirstOrDefault(fact => fact.Label == "Machine")?.Value ?? Environment.MachineName;

            ReplaceWith(OverviewCards, snapshot.OverviewCards);
            ReplaceWith(SystemFacts, snapshot.SystemFacts);
            ReplaceWith(DriveSnapshots, snapshot.DriveSnapshots);
            ReplaceWith(NetworkSnapshots, snapshot.NetworkSnapshots);
            ReplaceWith(RecentFiles, recentFilesTask.Result);
            ReplaceWith(RecentFolders, recentFoldersTask.Result);

            var launchers = launchersTask.Result;
            ReplaceWith(AppLaunchers, launchers);
            ReplaceWith(PinnedLaunchers, launchers.Take(4));

            LastRefreshLabel = $"Updated {DateTime.Now:MMM d, yyyy 'at' HH:mm:ss}";
            StatusMessage = "Dashboard ready";
        }
        catch (Exception exception)
        {
            StatusMessage = $"Refresh failed: {exception.Message}";
        }
    }

    private void UpdateClock()
    {
        var now = DateTime.Now;
        CurrentTime = now.ToString("HH:mm:ss");
        CurrentDate = now.ToString("dddd, MMMM d, yyyy");
    }

    private void Launch(AppLauncher? launcher)
    {
        if (launcher is null)
        {
            return;
        }

        try
        {
            _launcherService.Launch(launcher);
            StatusMessage = $"{launcher.Name} opened.";
        }
        catch (Exception exception)
        {
            StatusMessage = $"Could not open {launcher.Name}: {exception.Message}";
        }
    }

    private void OpenRecentFile(RecentFileItem? item)
    {
        if (item is null)
        {
            return;
        }

        try
        {
            _launcherService.OpenPath(item.LaunchPath);
            StatusMessage = $"{item.DisplayName} opened.";
        }
        catch (Exception exception)
        {
            StatusMessage = $"Could not open {item.DisplayName}: {exception.Message}";
        }
    }

    private void OpenRecentFolder(RecentFolderItem? item)
    {
        if (item is null)
        {
            return;
        }

        try
        {
            _launcherService.OpenPath(item.FullPath);
            StatusMessage = $"{item.DisplayName} opened.";
        }
        catch (Exception exception)
        {
            StatusMessage = $"Could not open {item.DisplayName}: {exception.Message}";
        }
    }

    private static void ReplaceWith<T>(ObservableCollection<T> target, IEnumerable<T> items)
    {
        target.Clear();

        foreach (var item in items)
        {
            target.Add(item);
        }
    }
}
