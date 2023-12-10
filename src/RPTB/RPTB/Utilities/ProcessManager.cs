using System.Diagnostics;

namespace RPTB.Utilities;

public static class ProcessManager
{
    private static Process? _caddyProcess;

    private static string GetExePath()
    {
        return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "caddy.exe");
    }

    public static void StartCaddyProcess()
    {
        _caddyProcess = Process.Start(GetProcessStartInfo());
    }

    public static void StopCaddyProcess()
    {
        _caddyProcess?.CloseMainWindow();
    }

    public static void RestartCaddyProcess()
    {
        StopCaddyProcess();
        StartCaddyProcess();
    }

    private static ProcessStartInfo GetProcessStartInfo()
    {
        return new ProcessStartInfo(GetExePath())
        {
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            Arguments = "run --config ./Caddyfile --adapter caddyfile"
        };
    }
}