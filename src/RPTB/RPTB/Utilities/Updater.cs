using System.IO.Compression;

namespace RPTB.Utilities;

internal static class Updater
{
    public static async Task DownloadCaddyPortableAsync()
    {
        Console.Clear();
        Console.WriteLine("Updating Caddy...");

        const string caddyDownloadUrl =
            "https://github.com/caddyserver/caddy/releases/latest/download/caddy_2.7.6_windows_amd64.zip";
        const string downloadedZipPath = "caddy.zip";
        using var client = new HttpClient();

        var response = await client.GetAsync(new Uri(caddyDownloadUrl));
        await using var contentStream = await response.Content.ReadAsStreamAsync();

        await using (var fileStream = new FileStream(downloadedZipPath, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            var totalRead = 0L;
            var buffer = new byte[8192];
            var isMoreToRead = true;

            if (response.Content.Headers.ContentLength != null)
            {
                var totalBytes = response.Content.Headers.ContentLength.Value;
                do
                {
                    var read = await contentStream.ReadAsync(buffer);
                    if (read == 0)
                    {
                        isMoreToRead = false;
                    }
                    else
                    {
                        await fileStream.WriteAsync(buffer.AsMemory(0, read));
                        totalRead += read;
                        var numChars = (int)Math.Ceiling(totalRead / (totalBytes / 40.0));
                        var percentage = (int)Math.Round((double)(totalRead * 100) / totalBytes);
                        Console.Write("\r[{0}{1}] {2}%", new string('#', numChars), new string(' ', 40 - numChars),
                            percentage);
                    }
                } while (isMoreToRead);
            }
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
}