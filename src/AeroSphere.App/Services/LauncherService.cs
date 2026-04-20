using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using AeroSphere.App.Models;

namespace AeroSphere.App.Services;

public sealed class LauncherService : ILauncherService
{
    public IReadOnlyList<AppLauncher> GetLaunchers()
    {
        var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var downloadsFolder = Path.Combine(userProfile, "Downloads");

        return
        [
            new("File Explorer", "Browse local folders and drives.", "explorer.exe"),
            new("Documents", "Open your Documents folder.", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)),
            new("Downloads", "Open your Downloads folder.", downloadsFolder),
            new("Pictures", "Open your Pictures folder.", Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)),
            new("Notepad", "Quick plain text editing.", "notepad.exe"),
            new("Calculator", "Basic calculator and conversions.", "calc.exe"),
            new("Task Manager", "Inspect running apps and processes.", "taskmgr.exe"),
            new("Control Panel", "Open classic Windows settings.", "control.exe"),
        ];
    }

    public void Launch(AppLauncher launcher)
    {
        Open(launcher.Target, launcher.Arguments);
    }

    public void OpenPath(string path)
    {
        Open(path);
    }

    private static void Open(string target, string? arguments = null)
    {
        if (string.IsNullOrWhiteSpace(target))
        {
            throw new InvalidOperationException("The launch target is empty.");
        }

        if (Path.IsPathRooted(target) && !File.Exists(target) && !Directory.Exists(target))
        {
            throw new FileNotFoundException("The requested path no longer exists.", target);
        }

        Process.Start(new ProcessStartInfo
        {
            FileName = target,
            Arguments = arguments ?? string.Empty,
            UseShellExecute = true,
        });
    }
}
