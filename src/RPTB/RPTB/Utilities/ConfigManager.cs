using System.Text;
using System.Text.RegularExpressions;

namespace RPTB.Utilities;

public static class ConfigManager
{
    private const string FilePath = "./caddyfile";

    public static void CreateConfig()
    {
        try
        {
            var caddyFileContent = GetCaddyFileContent();

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
            var newEntry = GetNewEntry();

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

        // Include a newline at the beginning if the file is not empty
        var newlinePrefix = File.Exists(FilePath) ? Environment.NewLine : string.Empty;

        return $"{newlinePrefix}{subdomain} {{\n    reverse_proxy {ipAddressAndPort}\n}}";
    }


    private static void FormatCaddyfile()
    {
        try
        {
            // Read the content of the file
            var content = File.ReadAllText(FilePath);

            // Use a regular expression to ensure only one newline between entries
            var pattern = $@"{Regex.Escape(Environment.NewLine)}{{2,}}";
            var replacement = Environment.NewLine + Environment.NewLine;
            var formattedContent = Regex.Replace(content, pattern, replacement);

            // Write the formatted content back to the file
            File.WriteAllText(FilePath, formattedContent);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error formatting Caddyfile: {ex.Message}");
        }
    }

    public static void ListEntries()
    {
        try
        {
            // Clear the console
            Console.Clear();

            // Read the content of the Caddyfile
            var content = File.ReadAllText(FilePath);

            // Use a regular expression to match all entries in the Caddyfile
            var entryRegex = new Regex(@"(?<entry>(?<subdomain>[\w.-]+)\s*{\s*reverse_proxy\s*(?<ip>[\w.:]+)\s*})");
            var matches = entryRegex.Matches(content);

            // Loop through each match
            foreach (Match match in matches)
            {
                // Extract the entire entry, the subdomain and the IP address
                var entry = match.Groups["entry"].Value;
                var subdomain = match.Groups["subdomain"].Value;
                var ip = match.Groups["ip"].Value;

                // Print the subdomain and the IP address
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
            // Read the content of the Caddyfile
            var content = File.ReadAllText(FilePath);

            // Use a regular expression to match all entries in the Caddyfile
            var entryRegex = new Regex(@"(?<entry>(?<subdomain>[\w.-]+)\s*{\s*reverse_proxy\s*(?<ip>[\w.:]+)\s*})");
            var matches = entryRegex.Matches(content);

            // Create a dictionary to store entries and their indices
            var entries = new Dictionary<int, string>();

            // Loop through each match
            for (var i = 0; i < matches.Count; i++)
            {
                var match = matches[i];

                // Extract the entire entry, the subdomain, and the IP address
                var entry = match.Groups["entry"].Value;
                var subdomain = match.Groups["subdomain"].Value;
                var ip = match.Groups["ip"].Value;

                // Store the entry in the dictionary with its index
                entries.Add(i + 1, entry);

                // Print the index, subdomain, and the IP address
                Console.WriteLine($"{i + 1}. {subdomain} : {ip}");
            }

            // Prompt user to select an entry
            Console.Write("Select the entry to update (enter the number): ");
            if (int.TryParse(Console.ReadLine(), out var selectedIndex) && entries.ContainsKey(selectedIndex))
            {
                // Get the selected entry
                var selectedEntry = entries[selectedIndex];

                // Prompt user for updated values
                Console.Write($"Enter new subdomain ({selectedEntry}): ");
                var newSubdomain = Console.ReadLine();

                Console.Write($"Enter new IP address and port ({selectedEntry}): ");
                var newIpAddressAndPort = Console.ReadLine();

                // Replace old values with new values
                var updatedEntry = selectedEntry
                    .Replace($"{{\n    reverse_proxy {selectedEntry}\n}}",
                        $"{{\n    reverse_proxy {newIpAddressAndPort}\n}}")
                    .Replace($"{selectedEntry}", $"{newSubdomain} {{\n    reverse_proxy {newIpAddressAndPort}\n}}");

                // Replace the old entry with the updated entry in the content
                content = content.Replace(selectedEntry, updatedEntry);

                // Write the updated content back to the file
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
            // Read the content of the Caddyfile
            var content = File.ReadAllText(FilePath);

            // Use a regular expression to match all entries in the Caddyfile
            var entryRegex = new Regex(@"(?<entry>(?<subdomain>[\w.-]+)\s*{\s*reverse_proxy\s*(?<ip>[\w.:]+)\s*})");
            var matches = entryRegex.Matches(content);

            // Create a dictionary to store entries and their indices
            var entries = new Dictionary<int, string>();

            // Loop through each match
            for (var i = 0; i < matches.Count; i++)
            {
                var match = matches[i];

                // Extract the entire entry, the subdomain, and the IP address
                var entry = match.Groups["entry"].Value;
                var subdomain = match.Groups["subdomain"].Value;
                var ip = match.Groups["ip"].Value;

                // Store the entry in the dictionary with its index
                entries.Add(i + 1, entry);

                // Print the index, subdomain, and the IP address
                Console.WriteLine($"{i + 1}. {subdomain} : {ip}");
            }

            // Prompt user to select an entry
            Console.Write("Select the entry to delete (enter the number): ");
            if (int.TryParse(Console.ReadLine(), out var selectedIndex) && entries.ContainsKey(selectedIndex))
            {
                // Get the selected entry
                var selectedEntry = entries[selectedIndex];

                // Replace the old entry with an empty string in the content
                content = content.Replace(selectedEntry, string.Empty);

                // Write the updated content back to the file
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
            // Read the content of the Caddyfile
            var content = File.ReadAllText(FilePath);

            // Use a regular expression to match all entries in the Caddyfile
            var entryRegex = new Regex(@"(?<entry>(?<subdomain>[\w.-]+)\s*{\s*reverse_proxy\s*(?<ip>[\w.:]+)\s*})");
            var matches = entryRegex.Matches(content);

            // Flag to track if any invalid entries are found
            var isValid = true;

            // Create a StringBuilder to store the validated entries
            var validatedEntries = new StringBuilder();

            // Loop through each match
            foreach (Match match in matches)
            {
                // Extract the entire entry, the subdomain, and the IP address
                var entry = match.Groups["entry"].Value;
                var subdomain = match.Groups["subdomain"].Value;
                var ip = match.Groups["ip"].Value;

                // Validate the entry format
                if (string.IsNullOrWhiteSpace(subdomain) || string.IsNullOrWhiteSpace(ip))
                {
                    Console.WriteLine($"Invalid entry found: {entry}");
                    isValid = false;
                }
                else
                {
                    // Append the valid entry to the StringBuilder
                    validatedEntries.AppendLine(entry);
                }
            }

            if (isValid)
                Console.WriteLine("All entries in Caddyfile are valid.");
            else
                Console.WriteLine("Caddyfile contains one or more invalid entries. Please fix them.");

            // Print the validated entries
            Console.WriteLine("Validated Caddyfile content:");
            Console.WriteLine(validatedEntries.ToString());

            // Write the validated content back to the file
            File.WriteAllText(FilePath, validatedEntries.ToString());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error validating Caddyfile: {ex.Message}");
        }
    }
}