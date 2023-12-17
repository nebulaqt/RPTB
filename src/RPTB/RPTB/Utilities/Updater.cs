using System.Runtime.InteropServices;
using SharpCompress.Archives;

namespace RPTB.Utilities;

internal static class Updater
{
    public static async Task DownloadCaddyPortableAsync()
    {
        Console.Clear();
        Console.WriteLine("Updating Caddy...");

        string caddyDownloadUrl;
        string downloadedZipPath;
        string caddyExecutablePath;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            caddyDownloadUrl =
                "https://github.com/caddyserver/caddy/releases/latest/download/caddy_2.7.6_windows_amd64.zip";
            downloadedZipPath = "caddy.zip";
            caddyExecutablePath = "./caddy.exe";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            caddyDownloadUrl =
                "https://github.com/caddyserver/caddy/releases/latest/download/caddy_2.7.6_linux_amd64.tar.gz";
            downloadedZipPath = "caddy.tar.gz";
            caddyExecutablePath = "./caddy";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            caddyDownloadUrl =
                "https://github.com/caddyserver/caddy/releases/latest/download/caddy_2.7.6_mac_amd64.tar.gz";
            downloadedZipPath = "caddy.tar.gz";
            caddyExecutablePath = "./caddy";
        }
        else
        {
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
                new FileStream(downloadedZipPath, FileMode.Create, FileAccess.Write, FileShare.None);

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

        try
        {
            ExtractArchive(downloadedZipPath, caddyExecutablePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error occurred during extraction: {ex.Message}");
        }
        finally
        {
            try
            {
                File.Delete(downloadedZipPath);
            }
            catch (IOException ioEx)
            {
                Console.WriteLine($"Error occurred during file cleanup: {ioEx.Message}");
                Console.ReadKey();
            }
        }
    }

    private static void DrawProgressBar(long totalBytesRead, long totalBytes)
    {
        var numChars = (int)Math.Ceiling(totalBytesRead / (totalBytes / 40.0));
        var percentage = (int)Math.Round((double)(totalBytesRead * 100) / totalBytes);
        Console.Write("\r[{0} {1}] {2}%", new string('#', numChars), new string(' ', 40 - numChars), percentage);
    }

    private static void ExtractArchive(string archivePath, string extractPath)
    {
        using var archive = ArchiveFactory.Open(archivePath);

        var entryKey = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "caddy.exe" : "caddy";
        var entry = archive.Entries.FirstOrDefault(e => e.Key.Equals(entryKey, StringComparison.OrdinalIgnoreCase));

        entry?.WriteTo(new FileStream(extractPath, FileMode.Create));
    }
}