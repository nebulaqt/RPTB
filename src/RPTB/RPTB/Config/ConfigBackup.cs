using System.IO.Compression;

namespace RPTB.Config
{
    internal class ConfigBackup
    {
        public static void BackupProxyDataFolder()
        {
            string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string proxyDataPath = Path.Combine(exeDirectory, "proxydata");

            if (!Directory.Exists(proxyDataPath))
            {
                Console.WriteLine("proxydata folder does not exist.");
                return;
            }

            string backupFileName = $"backup_time_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.zip";
            string backupFilePath = Path.Combine(exeDirectory, backupFileName);

            try
            {
                CompressionLevel compressionLevel = CompressionLevel.Optimal;
                ZipFile.CreateFromDirectory(proxyDataPath, backupFilePath, compressionLevel, false);
                Console.WriteLine($"Backup created successfully: {backupFileName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating backup: {ex.Message}");
            }
        }
    }
}
