namespace RPTB.Process;

public static class ProcessMonitor
{
    private static readonly string _uniqueTitle = "RPTB - Caddy";

    private static CancellationTokenSource? _cancellationTokenSource;

    public static async Task StartMonitoring()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        await MonitorProcess(_cancellationTokenSource.Token);
    }

    public static void StopMonitoring()
    {
        _cancellationTokenSource?.Cancel();
    }

    private static async Task MonitorProcess(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            if (!IsProcessRunning(_uniqueTitle))
            {
                Console.WriteLine("Caddy process is not running. Starting it now.");
                ProcessManager.StartCaddyProcess();
            }
            else
            {
                Console.WriteLine("Caddy process is already running.");
            }

            // Wait for some time before checking again
            await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken); // Adjust the time interval as needed
        }
    }


    private static bool IsProcessRunning(string uniqueTitle)
    {
        var processes = System.Diagnostics.Process.GetProcesses();
        foreach (var process in processes)
            if (ProcessManager.IsCaddyProcess(process))
                return true;
        return false;
    }
}