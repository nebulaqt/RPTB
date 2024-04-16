using System.Diagnostics;

namespace RPTB.Process;

public static class ProcessManager
{
    private const string ProcessName = "cmd.exe";
    private const string UniqueTitle = "RPTB - Caddy";
    private static readonly string CaddyPath = Path.Combine("proxydata", "caddy.exe");

    public static void StartCaddyProcess()
    {
        try
        {
            System.Diagnostics.Process.Start(GetProcessStartInfo());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to start Caddy process: {ex.Message}");
        }
    }

    public static void StopCaddyProcess()
    {
        try
        {
            var processes = System.Diagnostics.Process.GetProcesses();
            foreach (var process in processes)
            {
                if (!IsCaddyProcess(process)) continue;
                process.Kill();
                process.WaitForExit();
                process.Dispose();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to stop Caddy process: {ex.Message}");
        }
    }

    public static bool IsCaddyProcess(System.Diagnostics.Process process)
    {
        try
        {
            return !string.IsNullOrEmpty(process.MainWindowTitle) && process.MainWindowTitle.Contains(UniqueTitle);
        }
        catch (Exception)
        {
            // Ignore exceptions and return false if accessing the MainWindowTitle fails
            return false;
        }
    }


    public static void RestartCaddyProcess()
    {
        StopCaddyProcess();
        StartCaddyProcess();
    }

    private static ProcessStartInfo GetProcessStartInfo()
    {
        return new ProcessStartInfo(ProcessName)
        {
            UseShellExecute = true,
            Arguments = $"/k title {UniqueTitle} && {CaddyPath} run", // Set the window title and execute Caddy
            WindowStyle = ProcessWindowStyle.Normal,
            CreateNoWindow = false,
            RedirectStandardOutput = false,
            RedirectStandardError = false
        };
    }
}