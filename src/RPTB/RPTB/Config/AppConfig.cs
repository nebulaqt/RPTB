using System.IO;
using Newtonsoft.Json;

namespace RPTB.Config
{
    public class AppConfig
    {
        public string SelectedOperatingSystem { get; set; }

        // Add other configuration properties as needed

        public static AppConfig? Load(string filePath)
        {
            if (!File.Exists(filePath)) return new AppConfig();
            try
            {
                var json = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<AppConfig>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading config: {ex.Message}");
            }

            // Return a new AppConfig instance if the file doesn't exist or there's an error
            return new AppConfig();
        }

        public void Save(string filePath)
        {
            var json = JsonConvert.SerializeObject(this, Formatting.Indented);

            try
            {
                File.WriteAllText(filePath, json);
                Console.WriteLine("AppConfig saved successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving config: {ex.Message}");
            }
        }
    }
}