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
        var startInfo = new ProcessStartInfo(GetExePath())
        {
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        _caddyProcess = Process.Start(startInfo);
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
}