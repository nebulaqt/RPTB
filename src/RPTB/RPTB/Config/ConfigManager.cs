using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace RPTB.Config;

public static class ConfigManager
{
    private const string FilePath = "./caddyfile";

    public static void CreateConfig()
    {
        try
        {
            var caddyFileContent = GetCaddyFileContent();
            File.WriteAllText(FilePath, caddyFileContent);
            ValidateCaddyfile();
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
            var newEntry = GetNewEntry();
            File.AppendAllText(FilePath, newEntry);
            ValidateCaddyfile();
            Console.WriteLine("Custom text added as a new entry to Caddyfile successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating Caddyfile: {ex.Message}");
        }
    }

    private static string GetCaddyFileContent()
    {
        var caddyFileContent = new StringBuilder();
        do
        {
            Console.Write("Enter subdomain (e.g., sub.domain.tld): ");
            var subdomain = Console.ReadLine();
            Console.Write("Enter IP address and port (e.g., 127.0.0.1:8095): ");
            var ipAddressAndPort = Console.ReadLine();
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
        var subdomain = Console.ReadLine();
        Console.Write("Enter IP address and port (e.g., 127.0.0.1:8095): ");
        var ipAddressAndPort = Console.ReadLine();
        var newlinePrefix = File.Exists(FilePath) ? Environment.NewLine : string.Empty;
        return $"{newlinePrefix}{subdomain} {{\n    reverse_proxy {ipAddressAndPort}\n}}";
    }

    public static void ListEntries()
    {
        try
        {
            Console.Clear();
            var content = File.ReadAllText(FilePath);
            var entryRegex = new Regex(@"(?<entry>(?<subdomain>[\w.-]+)\s*{\s*reverse_proxy\s*(?<ip>[\w.:]+)\s*})");
            var matches = entryRegex.Matches(content);
            foreach (Match match in matches)
            {
                var subdomain = match.Groups["subdomain"].Value;
                var ip = match.Groups["ip"].Value;
                Console.WriteLine($"{subdomain} : {ip}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading Caddyfile: {ex.Message}");
        }
    }

    public static void UpdateEntry()
    {
        try
        {
            var content = File.ReadAllText(FilePath);
            var entryRegex = new Regex(@"(?<entry>(?<subdomain>[\w.-]+)\s*{\s*reverse_proxy\s*(?<ip>[\w.:]+)\s*})");
            var matches = entryRegex.Matches(content);
            var entries = new Dictionary<int, string>();
            for (var i = 0; i < matches.Count; i++)
            {
                var match = matches[i];
                var entry = match.Groups["entry"].Value;
                var subdomain = match.Groups["subdomain"].Value;
                var ip = match.Groups["ip"].Value;
                entries.Add(i + 1, entry);
                Console.WriteLine($"{i + 1}. {subdomain} : {ip}");
            }

            Console.Write("Select the entry to update (enter the number): ");
            if (int.TryParse(Console.ReadLine(), out var selectedIndex) &&
                entries.TryGetValue(selectedIndex, out var value))
            {
                Console.Write($"Enter new subdomain ({value}): ");
                var newSubdomain = Console.ReadLine();
                Console.Write($"Enter new IP address and port ({value}): ");
                var newIpAddressAndPort = Console.ReadLine();
                var updatedEntry = value
                    .Replace($"{{\n    reverse_proxy {value}\n}}",
                        $"{{\n    reverse_proxy {newIpAddressAndPort}\n}}")
                    .Replace($"{value}", $"{newSubdomain} {{\n    reverse_proxy {newIpAddressAndPort}\n}}");
                content = content.Replace(value, updatedEntry);
                File.WriteAllText(FilePath, content);

                Console.WriteLine("Entry updated successfully.");
            }
            else
            {
                Console.WriteLine("Invalid selection. No entry updated.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating entry: {ex.Message}");
        }
    }

    public static void DeleteEntry()
    {
        try
        {
            var content = File.ReadAllText(FilePath);
            var entryRegex = new Regex(@"(?<entry>(?<subdomain>[\w.-]+)\s*{\s*reverse_proxy\s*(?<ip>[\w.:]+)\s*})");
            var matches = entryRegex.Matches(content);
            var entries = new Dictionary<int, string>();
            for (var i = 0; i < matches.Count; i++)
            {
                var match = matches[i];
                var entry = match.Groups["entry"].Value;
                var subdomain = match.Groups["subdomain"].Value;
                var ip = match.Groups["ip"].Value;
                entries.Add(i + 1, entry);
                Console.WriteLine($"{i + 1}. {subdomain} : {ip}");
            }

            Console.Write("Select the entry to delete (enter the number): ");
            if (int.TryParse(Console.ReadLine(), out var selectedIndex) &&
                entries.TryGetValue(selectedIndex, out var value))
            {
                content = content.Replace(value, string.Empty);
                File.WriteAllText(FilePath, content);
                Console.WriteLine("Entry deleted successfully.");
            }
            else
            {
                Console.WriteLine("Invalid selection. No entry deleted.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting entry: {ex.Message}");
        }
    }

    public static void ValidateCaddyfile()
    {
        try
        {
            var process = new System.Diagnostics.Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "caddy",
                    Arguments = "fmt ./caddyfile -w",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };

            process.Start();

            process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();

            process.WaitForExit();

            Console.WriteLine(process.ExitCode == 0
                ? "Caddyfile is valid and formatted successfully."
                : $"Error validating and formatting Caddyfile: {error}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error validating and formatting Caddyfile: {ex.Message}");
        }
    }
}