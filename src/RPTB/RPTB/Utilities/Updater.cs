using System.IO.Compression;

namespace RPTB.Utilities;

internal static class Updater
{
    public static async Task DownloadCaddyPortableAsync()
    {
        Console.Clear();
        Console.WriteLine("Updating Caddy...");
        const string caddyDownloadUrl = "https://github.com/caddyserver/caddy/releases/latest/download/caddy_2.7.6_windows_amd64.zip";
        const string downloadedZipPath = "caddy.zip";
        using var client = new HttpClient();
        var response = await client.GetAsync(new Uri(caddyDownloadUrl));
        await using var contentStream = await response.Content.ReadAsStreamAsync();
        await using var fileStream = new FileStream(downloadedZipPath, FileMode.Create, FileAccess.Write, FileShare.None);
        
        const int bufferSize = 8192;
        var buffer = new byte[bufferSize];
        long totalBytesRead = 0;

        if (response.Content.Headers.ContentLength != null)
        {
            var totalBytes = response.Content.Headers.ContentLength.Value;
            
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

        try
        {
            using var zipArchive = ZipFile.OpenRead(downloadedZipPath);
            var caddyEntry = zipArchive.GetEntry("caddy.exe");
            caddyEntry?.ExtractToFile("./caddy.exe", true);
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
}