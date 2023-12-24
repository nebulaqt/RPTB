using System.Diagnostics;

namespace RPTB.Process;

public static class ProcessManager
{
    private const string UniqueTitle = "RPTB - Caddy";

    private static string GetExePath(string? selectedOperatingSystem)
    {
        var exeFileName = selectedOperatingSystem switch
        {
            "w" => "caddy.exe",
            "l" => "caddy",
            "m" => "caddy",
            _ => throw new ArgumentException("Unsupported operating system.")
        };

        return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, exeFileName);
    }

    public static void StartCaddyProcess(string? selectedOperatingSystem)
    {
        System.Diagnostics.Process.Start(GetProcessStartInfo(selectedOperatingSystem));
    }

    public static void StopCaddyProcess()
    {
        var localAll = System.Diagnostics.Process.GetProcesses();
        foreach (var p in localAll)
        {
            if (string.IsNullOrEmpty(p.MainWindowTitle) || !p.MainWindowTitle.Contains(UniqueTitle)) continue;
            p.Kill();
            p.WaitForExit();
        }
    }

    public static void RestartCaddyProcess(string? selectedOperatingSystem)
    {
        StopCaddyProcess();
        StartCaddyProcess(selectedOperatingSystem);
    }

    private static ProcessStartInfo GetProcessStartInfo(string? selectedOperatingSystem)
    {
        var exePath = GetExePath(selectedOperatingSystem);

        // Check the selected operating system
        switch (selectedOperatingSystem)
        {
            case "w":
                // Start Caddy on Windows
                return new ProcessStartInfo("cmd.exe")
                {
                    UseShellExecute = true,
                    Arguments = $"/C title {UniqueTitle} && {exePath} run"
                };

            case "l":
            case "m":
                // Start Caddy in a screen session on Linux or macOS
                const string screenPath = "/usr/bin/screen"; // Adjust the path based on your system
                return new ProcessStartInfo(screenPath, $"-S caddyrptb -dm {exePath} run")
                {
                    UseShellExecute = false
                };

            default:
                throw new ArgumentException("Unsupported operating system.");
        }
    }

}