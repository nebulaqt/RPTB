using System;
using System.Diagnostics;
using System.IO;

namespace RPTB.ProcUtilities
{
    public static class ProcessManager
    {
        private const string ProcessName = "cmd.exe";
        private static string _caddyPath = Path.Combine("proxydata", "caddy.exe");
        private static string _arguments = $"/C \"{_caddyPath}\" run";
        private const string UniqueTitle = "RPTB - Caddy";

        public static void StartCaddyProcess()
        {
            try
            {
                Process.Start(GetProcessStartInfo());
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
                var processes = Process.GetProcesses();
                foreach (var process in processes)
                {
                    if (IsCaddyProcess(process))
                    {
                        process.Kill();
                        process.WaitForExit();
                        process.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to stop Caddy process: {ex.Message}");
            }
        }

        public static bool IsCaddyProcess(Process process)
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
                Arguments = $"/k title {UniqueTitle} && {_caddyPath} run", // Set the window title and execute Caddy
                WindowStyle = ProcessWindowStyle.Normal,
                CreateNoWindow = false,
                RedirectStandardOutput = false,
                RedirectStandardError = false
            };
        }
    }
}
