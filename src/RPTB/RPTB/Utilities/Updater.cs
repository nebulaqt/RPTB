using System.Diagnostics;

namespace RPTB.Utilities;

internal static class Updater
{
    public static async Task DownloadCaddyPortableAsync(string? selectedOperatingSystem)
    {
        Console.Clear();
        Console.WriteLine("Updating Caddy...");

        string caddyDownloadUrl;
        string downloadedFilePath;

        switch (selectedOperatingSystem)
        {
            case "w":
                caddyDownloadUrl =
                    "https://github.com/nebulaqt/RPTB-Storage/raw/main/win/caddy.exe";
                downloadedFilePath = "caddy.exe";
                break;
            case "l":
                caddyDownloadUrl =
                    "https://github.com/nebulaqt/RPTB-Storage/raw/main/linux/caddy";
                downloadedFilePath = "caddy";
                break;
            case "m":
                caddyDownloadUrl =
                    "https://github.com/nebulaqt/RPTB-Storage/raw/main/mac/caddy";
                downloadedFilePath = "caddy";
                break;
            default:
                Console.WriteLine("Unsupported operating system.");
                return;
        }

        using var client = new HttpClient();

        try
        {
            using var response = await client.GetAsync(new Uri(caddyDownloadUrl));
            response.EnsureSuccessStatusCode();

            await using var contentStream = await response.Content.ReadAsStreamAsync();

            await using var fileStream =
                new FileStream(downloadedFilePath, FileMode.Create, FileAccess.Write, FileShare.None);

            const int bufferSize = 8192;
            var buffer = new byte[bufferSize];
            long totalBytesRead = 0;

            var totalBytes = response.Content.Headers.ContentLength ?? 0;

            int bytesRead;
            do
            {
                bytesRead = await contentStream.ReadAsync(buffer);
                if (bytesRead == 0) continue;
                await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead));
                totalBytesRead += bytesRead;
                DrawProgressBar(totalBytesRead, totalBytes);
            } while (bytesRead != 0);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error occurred during download: {ex.Message}");
            return;
        }

        if (selectedOperatingSystem == "l")
            // Set executable permission for Linux.
            SetExecutablePermissionForLinux(downloadedFilePath);
    }

    private static void DrawProgressBar(long totalBytesRead, long totalBytes)
    {
        var numChars = (int)Math.Ceiling(totalBytesRead / (totalBytes / 40.0));
        var percentage = (int)Math.Round((double)(totalBytesRead * 100) / totalBytes);
        Console.Write("\r[{0} {1}] {2}%", new string('#', numChars), new string(' ', 40 - numChars), percentage);
    }

    private static void SetExecutablePermissionForLinux(string filePath)
    {
        try
        {
            // Use the 'bash' command to execute 'chmod' within WSL.
            var processInfo = new ProcessStartInfo
            {
                FileName = "bash",
                Arguments = $"-c \"chmod +x {filePath}\"",
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process();
            process.StartInfo = processInfo;
            process.Start();
            process.WaitForExit();

            if (process.ExitCode != 0)
                Console.WriteLine($"Failed to set executable permission. Exit code: {process.ExitCode}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error occurred while setting executable permission: {ex.Message}");
        }
    }
}