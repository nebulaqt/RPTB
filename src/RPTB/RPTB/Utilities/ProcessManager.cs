using System.Diagnostics;

namespace RPTB.Utilities;

public static class ProcessManager
{
    private const string UniqueTitle = "RPTB - Caddy";

    private static string GetExePath()
    {
        return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "caddy.exe");
    }

    public static void StartCaddyProcess()
    {
        Process.Start(GetProcessStartInfo());
    }

    public static void StopCaddyProcess()
    {
        var localAll = Process.GetProcesses();
        foreach (var p in localAll)
        {
            if (string.IsNullOrEmpty(p.MainWindowTitle) || !p.MainWindowTitle.Contains(UniqueTitle)) continue;
            p.Kill();
            p.WaitForExit();
        }
    }

    public static void RestartCaddyProcess()
    {
        StopCaddyProcess();
        StartCaddyProcess();
    }

    private static ProcessStartInfo GetProcessStartInfo()
    {
        return new ProcessStartInfo("cmd.exe")
        {
            UseShellExecute = true,
            Arguments = $"/K title {UniqueTitle} && {GetExePath()} run"
        };
    }
}