using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using AeroSphere.App.Models;

namespace AeroSphere.App.Services;

public sealed class SystemSnapshotService : ISystemSnapshotService
{
    public DashboardSnapshot GetSnapshot()
    {
        var drives = DriveInfo.GetDrives()
            .Where(drive => drive.DriveType == DriveType.Fixed && drive.IsReady)
            .Select(drive => new DriveSnapshot(
                drive.Name,
                FormatBytes(drive.AvailableFreeSpace),
                FormatBytes(drive.TotalSize),
                $"{BuildUsagePercent(drive)} used"))
            .ToList();

        var networks = NetworkInterface.GetAllNetworkInterfaces()
            .Where(adapter =>
                adapter.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                adapter.NetworkInterfaceType != NetworkInterfaceType.Tunnel)
            .Select(adapter => new NetworkSnapshot(
                adapter.Name,
                adapter.OperationalStatus.ToString(),
                BuildAddressText(adapter)))
            .OrderBy(snapshot => snapshot.Status != nameof(OperationalStatus.Up))
            .ThenBy(snapshot => snapshot.Name)
            .ToList();

        var systemFacts = new List<SystemFact>
        {
            new("Machine", Environment.MachineName),
            new("User", Environment.UserName),
            new("Operating system", RuntimeInformation.OSDescription),
            new("Framework", RuntimeInformation.FrameworkDescription),
            new("Architecture", $"{RuntimeInformation.OSArchitecture} OS / {RuntimeInformation.ProcessArchitecture} process"),
            new("Application", Environment.ProcessPath ?? "AeroSphere.App.exe"),
            new("Uptime", FormatUptime(Environment.TickCount64)),
            new("Current directory", Environment.CurrentDirectory),
        };

        var overviewCards = new List<OverviewCard>
        {
            new("Machine", Environment.MachineName, RuntimeInformation.OSDescription),
            new("User session", Environment.UserName, FormatUptime(Environment.TickCount64)),
            new("Storage", drives.FirstOrDefault()?.FreeSpaceLabel ?? "No fixed drives", drives.Count == 1 ? "1 ready drive" : $"{drives.Count} ready drives"),
            new("Network", networks.FirstOrDefault(snapshot => snapshot.Status == nameof(OperationalStatus.Up))?.AddressText ?? "Offline", networks.Any(snapshot => snapshot.Status == nameof(OperationalStatus.Up)) ? "Adapter online" : "No active adapter"),
        };

        return new DashboardSnapshot(
            "A stable Windows 10 desktop shell",
            "This build stays in standard .NET, uses classic WPF controls, reads only local machine data, and packages cleanly into a normal Windows installer.",
            overviewCards,
            systemFacts,
            drives,
            networks);
    }

    private static string BuildAddressText(NetworkInterface adapter)
    {
        try
        {
            var address = adapter.GetIPProperties()
                .UnicastAddresses
                .FirstOrDefault(item => item.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);

            return address?.Address.ToString() ?? "No IPv4 address";
        }
        catch
        {
            return "Unavailable";
        }
    }

    private static string BuildUsagePercent(DriveInfo drive)
    {
        var usedSpace = drive.TotalSize - drive.AvailableFreeSpace;
        var usage = drive.TotalSize == 0 ? 0 : (double)usedSpace / drive.TotalSize;
        return $"{usage:P0}";
    }

    private static string FormatBytes(long bytes)
    {
        const double bytesPerGigabyte = 1024d * 1024d * 1024d;
        return $"{bytes / bytesPerGigabyte:F1} GB";
    }

    private static string FormatUptime(long milliseconds)
    {
        var uptime = TimeSpan.FromMilliseconds(milliseconds);
        return $"{uptime.Days}d {uptime.Hours}h {uptime.Minutes}m";
    }
}
