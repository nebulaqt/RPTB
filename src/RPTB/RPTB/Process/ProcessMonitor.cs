using System.Diagnostics;

namespace RPTB.Process;

public static class ProcessMonitor
{
    private const string UniqueTitle = "RPTB - Caddy";

    public static void MonitorCaddyProcess(string? selectedOperatingSystem)
    {
        const string processName = "caddy";
        var isProcessRunning = IsProcessRunning(processName);

        if (!isProcessRunning)
        {
            Console.WriteLine($"{processName} process is not running. Starting it now.");

            switch (selectedOperatingSystem?.ToLower())
            {
                case "w":
                    StartProcessOnWindows(processName);
                    break;

                case "l":
                    StartProcessOnLinux(processName);
                    break;

                case "m":
                    StartProcessOnMacOS(processName);
                    break;

                default:
                    Console.WriteLine("Invalid operating system code.");
                    break;
            }
        }
        else
        {
            Console.WriteLine($"{processName} process is already running.");
        }
    }

    private static bool IsProcessRunning(string processName)
    {
        var processes = System.Diagnostics.Process.GetProcessesByName(processName);
        return processes.Length > 0;
    }

    private static void StartProcessOnWindows(string processName)
    {
        var startInfo = new ProcessStartInfo("cmd.exe")
        {
            UseShellExecute = true,
            Arguments = $"/C title {UniqueTitle} && caddy run"
        };

        System.Diagnostics.Process.Start(startInfo);

        Console.WriteLine($"{processName} process started on Windows with unique title.");
    }

    private static void StartProcessOnLinux(string processName)
    {
        System.Diagnostics.Process.Start("caddy", "run");
        Console.WriteLine($"{processName} process started on Linux.");
    }

    private static void StartProcessOnMacOS(string processName)
    {
        System.Diagnostics.Process.Start("caddy", "run");
        Console.WriteLine($"{processName} process started on macOS.");
    }
}