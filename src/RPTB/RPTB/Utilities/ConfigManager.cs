using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace RPTB.Utilities
{
    public class ConfigManager
    {
        private const string FilePath = "./caddyfile";

        public static void CreateConfig()
        {
            try
            {
                string caddyFileContent = GetCaddyFileContent();

                // Write the content to the file
                File.WriteAllText(FilePath, caddyFileContent);

                // Format the entire Caddyfile
                FormatCaddyfile();

                Console.WriteLine("Caddyfile created successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating Caddyfile: {ex.Message}");
            }
        }

        public static void AddEntry()
        {
            try
            {
                string newEntry = GetNewEntry();

                // Save the new entry to the file
                File.AppendAllText(FilePath, newEntry);

                // Format the entire Caddyfile
                FormatCaddyfile();

                Console.WriteLine("Custom text added as a new entry to Caddyfile successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating Caddyfile: {ex.Message}");
            }
        }

        private static string GetCaddyFileContent()
        {
            StringBuilder caddyFileContent = new StringBuilder();

            do
            {
                Console.Write("Enter subdomain (e.g., sub.domain.tld): ");
                string subdomain = Console.ReadLine();

                Console.Write("Enter IP address and port (e.g., 127.0.0.1:8095): ");
                string ipAddressAndPort = Console.ReadLine();

                caddyFileContent.AppendLine($"{subdomain} {{");
                caddyFileContent.AppendLine($"    reverse_proxy {ipAddressAndPort}");
                caddyFileContent.AppendLine("}");

                Console.Write("Do you want to add another entry? (y/n): ");
            } while (Console.ReadLine()?.ToLower() == "y");

            return caddyFileContent.ToString();
        }


        private static string GetNewEntry()
        {
            Console.Write("Enter subdomain (e.g., sub.domain.tld): ");
            string subdomain = Console.ReadLine();

            Console.Write("Enter IP address and port (e.g., 127.0.0.1:8095): ");
            string ipAddressAndPort = Console.ReadLine();

            return $"{subdomain} {{\n    reverse_proxy {ipAddressAndPort}\n}}";
        }

        private static void FormatCaddyfile()
        {
            try
            {
                // Read the content of the file
                string content = File.ReadAllText(FilePath);

                // Use a regular expression to ensure only one newline between entries
                string pattern = $@"{Regex.Escape(Environment.NewLine)}{{2,}}";
                string replacement = Environment.NewLine + Environment.NewLine;
                string formattedContent = Regex.Replace(content, pattern, replacement);

                // Write the formatted content back to the file
                File.WriteAllText(FilePath, formattedContent);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error formatting Caddyfile: {ex.Message}");
            }
        }
    }
}
