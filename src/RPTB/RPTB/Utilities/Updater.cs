using System.IO.Compression;

namespace RPTB.Utilities;

internal static class Updater
{
    public static async Task DownloadCaddyPortableAsync()
    {
        Console.Clear();
        Console.WriteLine("Updating Caddy...");

        const string downloadUrl =
            "https://github.com/caddyserver/caddy/releases/download/v2.7.6/caddy_2.7.6_windows_amd64.zip";
        const string downloadedFileName = "caddy.zip";
        const string extractionFolder = "proxydata";

        try
        {
            // Download Caddy zip file with progress
            using var client = new HttpClient();
            using var response = await client.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            var totalBytes = response.Content.Headers.ContentLength ?? -1;
            var bytesRead = 0L;

            // Create a new directory for extraction
            if (!Directory.Exists(extractionFolder)) Directory.CreateDirectory(extractionFolder);

            // Path to save downloaded file
            var downloadedFilePath = Path.Combine(extractionFolder, downloadedFileName);

            // Write the content to the file with progress
            await using var contentStream = await response.Content.ReadAsStreamAsync();
            await using var fileStream =
                new FileStream(downloadedFilePath, FileMode.Create, FileAccess.Write, FileShare.None);
            var buffer = new byte[8192];
            int read;
            while ((read = await contentStream.ReadAsync(buffer)) > 0)
            {
                await fileStream.WriteAsync(buffer.AsMemory(0, read));
                bytesRead += read;
                DrawProgressBar("Downloading", bytesRead, totalBytes);
            }

            // Close the file stream
            await fileStream.FlushAsync();
            fileStream.Close();

            // Unzip the downloaded file to extraction folder with progress
            using var archive = ZipFile.OpenRead(downloadedFilePath);
            foreach (var entry in archive.Entries)
            {
                var entryExtractPath = Path.Combine(extractionFolder, entry.FullName);
                entry.ExtractToFile(entryExtractPath, true);
                DrawProgressBar("Extracting", bytesRead, totalBytes);
            }

            // Close the archive
            archive.Dispose();

            // Delete the downloaded zip file
            File.Delete(downloadedFilePath);

            Console.WriteLine("\nCaddy update completed successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error occurred during update: {ex.Message}");
        }
    }

    private static void DrawProgressBar(string action, long bytesRead, long totalBytes)
    {
        var progressPercentage = totalBytes <= 0 ? 100 : (int)(bytesRead * 100 / totalBytes);
        var progressLength = Console.WindowWidth - 20; // Adjust according to your console window width
        var progressBar = new string('#', progressLength * progressPercentage / 100) +
                          new string('-', progressLength - progressLength * progressPercentage / 100);
        Console.Write($"\r{action} [{progressBar}] {progressPercentage}%");
    }
}