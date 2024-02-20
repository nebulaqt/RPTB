using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace RPTB.ProcUtilities
{
    public static class ProcessMonitor
    {
        private static string _uniqueTitle = "RPTB - Caddy";

        private static CancellationTokenSource? cancellationTokenSource;

        public static async Task StartMonitoring()
        {
            cancellationTokenSource = new CancellationTokenSource();
            await MonitorProcess(cancellationTokenSource.Token);
        }

        public static void StopMonitoring()
        {
            cancellationTokenSource?.Cancel();
        }

        private static async Task MonitorProcess(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (!IsProcessRunning(_uniqueTitle))
                {
                    Console.WriteLine($"Caddy process is not running. Starting it now.");
                    ProcessManager.StartCaddyProcess();
                }
                else
                {
                    Console.WriteLine($"Caddy process is already running.");
                }

                // Wait for some time before checking again
                await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken); // Adjust the time interval as needed
            }
        }


        private static bool IsProcessRunning(string uniqueTitle)
        {
            var processes = Process.GetProcesses();
            foreach (var process in processes)
            {
                if (ProcessManager.IsCaddyProcess(process))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
