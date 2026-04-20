using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AeroSphere.App.Models;

namespace AeroSphere.App.Services;

public sealed class RecentContentService : IRecentContentService
{
    public IReadOnlyList<RecentFileItem> GetRecentFiles(int maxItems)
    {
        var recentFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Microsoft",
            "Windows",
            "Recent");

        if (Directory.Exists(recentFolder))
        {
            return new DirectoryInfo(recentFolder)
                .EnumerateFiles("*.lnk", SearchOption.TopDirectoryOnly)
                .OrderByDescending(file => file.LastWriteTimeUtc)
                .Take(maxItems)
                .Select(file => new RecentFileItem(
                    Path.GetFileNameWithoutExtension(file.Name),
                    file.FullName,
                    "Windows Recent",
                    file.LastWriteTime.ToString("MMM d, yyyy HH:mm")))
                .ToList();
        }

        return EnumerateFallbackFiles(maxItems);
    }

    public IReadOnlyList<RecentFolderItem> GetRecentFolders(int maxItems)
    {
        var candidateFolders = new[]
        {
            Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads"),
            Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
        };

        var allFolders = new List<DirectoryInfo>();

        foreach (var folder in candidateFolders.Where(Directory.Exists))
        {
            var rootDirectory = new DirectoryInfo(folder);
            allFolders.Add(rootDirectory);

            try
            {
                allFolders.AddRange(rootDirectory.EnumerateDirectories());
            }
            catch
            {
                // Ignore folders we cannot enumerate and keep the dashboard responsive.
            }
        }

        return allFolders
            .GroupBy(directory => directory.FullName, StringComparer.OrdinalIgnoreCase)
            .Select(group => group.First())
            .OrderByDescending(directory => directory.LastWriteTimeUtc)
            .Take(maxItems)
            .Select(directory => new RecentFolderItem(
                directory.Name,
                directory.FullName,
                directory.LastWriteTime.ToString("MMM d, yyyy HH:mm"),
                BuildItemCountLabel(directory)))
            .ToList();
    }

    private static IReadOnlyList<RecentFileItem> EnumerateFallbackFiles(int maxItems)
    {
        var candidateFolders = new[]
        {
            Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads"),
            Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
        };

        var files = new List<FileInfo>();

        foreach (var folder in candidateFolders.Where(Directory.Exists))
        {
            try
            {
                files.AddRange(new DirectoryInfo(folder).EnumerateFiles("*", SearchOption.TopDirectoryOnly));
            }
            catch
            {
                // Ignore folders we cannot enumerate.
            }
        }

        return files
            .OrderByDescending(file => file.LastWriteTimeUtc)
            .Take(maxItems)
            .Select(file => new RecentFileItem(
                file.Name,
                file.FullName,
                file.Directory?.Name ?? "Local folder",
                file.LastWriteTime.ToString("MMM d, yyyy HH:mm")))
            .ToList();
    }

    private static string BuildItemCountLabel(DirectoryInfo directory)
    {
        try
        {
            var itemCount = directory.EnumerateFileSystemInfos().Take(250).Count();
            return itemCount == 1 ? "1 item" : $"{itemCount} items";
        }
        catch
        {
            return "Unavailable";
        }
    }
}
