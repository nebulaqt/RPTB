using System.Diagnostics;

namespace RPTB.Config;

public static class ConfigManager
{
    private static readonly string CaddyfilePath = Path.Combine("proxydata", "Caddyfile");

    public static void AddEntry()
    {
        Console.WriteLine("What type of entry do you want to add?");
        Console.WriteLine("1. Static File Server");
        Console.WriteLine("2. Reverse Proxy");

        int choice;
        while (!int.TryParse(Console.ReadLine(), out choice) || (choice != 1 && choice != 2))
            Console.WriteLine("Invalid choice. Please enter 1 or 2.");

        if (choice == 1)
            AddStaticFileServerEntry();
        else
            AddReverseProxyEntry();
    }

    public static void UpdateEntry()
    {
        Console.WriteLine("What type of entry do you want to update?");
        Console.WriteLine("1. Static File Server");
        Console.WriteLine("2. Reverse Proxy");

        int choice;
        while (!int.TryParse(Console.ReadLine(), out choice) || (choice != 1 && choice != 2))
            Console.WriteLine("Invalid choice. Please enter 1 or 2.");

        if (choice == 1)
            UpdateStaticFileServerEntry();
        else
            UpdateReverseProxyEntry();
    }

    public static void DeleteEntry()
    {
        Console.WriteLine("What type of entry do you want to delete?");
        Console.WriteLine("1. Static File Server");
        Console.WriteLine("2. Reverse Proxy");

        int choice;
        while (!int.TryParse(Console.ReadLine(), out choice) || (choice != 1 && choice != 2))
            Console.WriteLine("Invalid choice. Please enter 1 or 2.");

        if (choice == 1)
            DeleteStaticFileServerEntry();
        else
            DeleteReverseProxyEntry();
    }

    private static void AddStaticFileServerEntry()
    {
        try
        {
            Console.Write("Enter subdomain for static file server (e.g., dev.nobyl.net): ");
            var domain = Console.ReadLine();
            Console.Write("Enter root path for static files: ");
            var rootPath = Console.ReadLine();

            var entry = new Entry { Domain = domain, ProxyType = "file_server", ProxyDetails = rootPath };

            AppendEntriesToFile([entry]);
            ValidateCaddyfile();
            Console.WriteLine("Static file server added successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding static file server: {ex.Message}");
        }
    }

    private static void UpdateStaticFileServerEntry()
    {
        try
        {
            var entries = ReadEntriesFromFile();
            var staticFileServerEntries = entries.Where(entry => entry.ProxyType == "file_server").ToList();

            if (staticFileServerEntries.Any())
                ListAndSelectEntry(staticFileServerEntries, "update", (selectedEntries, selectedIndex) =>
                {
                    Console.Write("Enter new subdomain: ");
                    var newDomain = Console.ReadLine();
                    Console.Write("Enter new root path for static files: ");
                    var newRootPath = Console.ReadLine();

                    selectedEntries[selectedIndex].Domain = newDomain;
                    selectedEntries[selectedIndex].ProxyDetails = newRootPath;

                    WriteEntriesToFile(entries);
                    Console.WriteLine("Static file server entry updated successfully.");
                });
            else
                Console.WriteLine("No static file server entries found.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating static file server entry: {ex.Message}");
        }
    }


    private static void DeleteStaticFileServerEntry()
    {
        try
        {
            var entries = ReadEntriesFromFile();
            var staticFileServerEntries = entries.Where(entry => entry.ProxyType == "file_server").ToList();

            if (staticFileServerEntries.Count != 0)
                ListAndSelectEntry(staticFileServerEntries, "delete", (selectedEntries, selectedIndex) =>
                {
                    selectedEntries.RemoveAt(selectedIndex);
                    // Write all entries except the deleted static file server entry
                    var remainingEntries = entries.Where(entry => entry.ProxyType != "file_server").ToList();
                    WriteEntriesToFile(remainingEntries);
                    ValidateCaddyfile();
                    Console.WriteLine("Static file server entry deleted successfully.");
                });
            else
                Console.WriteLine("No static file server entries found.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting static file server entry: {ex.Message}");
        }
    }
    private static void AddReverseProxyEntry()
    {
        try
        {
            var entries = GetEntriesFromUser();
            AppendEntriesToFile(entries);
            ValidateCaddyfile();
            Console.WriteLine("Reverse proxy entry added successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding reverse proxy entry: {ex.Message}");
        }
    }

    private static void UpdateReverseProxyEntry()
    {
        try
        {
            var entries = ReadEntriesFromFile();
            var reverseProxyEntries = entries.Where(entry => entry.ProxyType == "reverse_proxy").ToList();

            if (reverseProxyEntries.Count != 0)
                ListAndSelectEntry(reverseProxyEntries, "update", (selectedEntries, selectedIndex) =>
                {
                    Console.Write("Enter new subdomain: ");
                    var newDomain = Console.ReadLine();
                    Console.Write("Enter new IP address and port: ");
                    var newReverseProxy = Console.ReadLine();

                    // Update only the selected reverse proxy entry
                    selectedEntries[selectedIndex].Domain = newDomain;
                    selectedEntries[selectedIndex].ProxyDetails = newReverseProxy;

                    WriteEntriesToFile(entries);
                    Console.WriteLine("Reverse proxy entry updated successfully.");
                });
            else
                Console.WriteLine("No reverse proxy entries found.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating reverse proxy entry: {ex.Message}");
        }
    }


    private static void DeleteReverseProxyEntry()
    {
        try
        {
            var entries = ReadEntriesFromFile();
            var reverseProxyEntries = entries.Where(entry => entry.ProxyType == "reverse_proxy").ToList();

            if (reverseProxyEntries.Count != 0)
                ListAndSelectEntry(reverseProxyEntries, "delete", (selectedEntries, selectedIndex) =>
                {
                    selectedEntries.RemoveAt(selectedIndex);
                    // Write all entries except the deleted reverse proxy entry
                    var remainingEntries = entries.Where(entry => entry.ProxyType != "reverse_proxy").ToList();
                    WriteEntriesToFile(remainingEntries);
                    ValidateCaddyfile();
                    Console.WriteLine("Reverse proxy entry deleted successfully.");
                });
            else
                Console.WriteLine("No reverse proxy entries found.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting reverse proxy entry: {ex.Message}");
        }
    }


    private static List<Entry> GetEntriesFromUser()
    {
        var entries = new List<Entry>();
        do
        {
            Console.Write("Enter subdomain (e.g., sub.domain.tld): ");
            var domain = Console.ReadLine();
            Console.Write("Enter IP address and port (e.g., 127.0.0.1:8095): ");
            var reverseProxy = Console.ReadLine();
            entries.Add(new Entry { Domain = domain, ProxyType = "reverse_proxy", ProxyDetails = reverseProxy });
            Console.Write("Do you want to add another entry? (y/n): ");
        } while (Console.ReadLine()?.Trim().ToLower() == "y");

        return entries;
    }

    private static void WriteEntriesToFile(List<Entry> entries)
    {
        using var writer = new StreamWriter(CaddyfilePath);
        foreach (var entry in entries) WriteEntryToFile(writer, entry);
    }

    private static void AppendEntriesToFile(List<Entry> entries)
    {
        using var writer = new StreamWriter(CaddyfilePath, true);
        foreach (var entry in entries) WriteEntryToFile(writer, entry);
    }

    private static void WriteEntryToFile(StreamWriter writer, Entry entry)
    {
        writer.WriteLine($"{entry.Domain} {{");
        if (entry.ProxyType == "file_server")
        {
            writer.WriteLine($"    root * {entry.ProxyDetails}");
            writer.WriteLine("    file_server");
        }
        else
        {
            writer.WriteLine($"    reverse_proxy {entry.ProxyDetails}");
        }

        writer.WriteLine("}");
    }

    private static List<Entry> ReadEntriesFromFile()
    {
        var entries = new List<Entry>();
        using var reader = new StreamReader(CaddyfilePath);
        Entry? currentEntry = null;

        while (reader.ReadLine() is { } line)
            if (line.Trim().EndsWith("{"))
            {
                currentEntry = new Entry { Domain = line.Trim().TrimEnd('{').Trim() };
            }
            else if (line.Trim() == "file_server")
            {
                if (currentEntry == null) continue;
                currentEntry.ProxyType = "file_server";
                entries.Add(currentEntry);
                currentEntry = null;
            }
            else if (line.Trim().StartsWith("root") && currentEntry != null)
            {
                var rootPath = line.Trim().Split(' ').LastOrDefault();
                currentEntry.ProxyType = "file_server";
                currentEntry.ProxyDetails = rootPath;
                entries.Add(currentEntry);
                currentEntry = null;
            }
            else if (line.Trim().StartsWith("reverse_proxy") && currentEntry != null)
            {
                var reverseProxy = line.Trim().Split(' ')[1];
                currentEntry.ProxyType = "reverse_proxy";
                currentEntry.ProxyDetails = reverseProxy;
                entries.Add(currentEntry);
                currentEntry = null;
            }

        return entries;
    }

    private static void ListAndSelectEntry(List<Entry> entries, string action, Action<List<Entry>, int> actionCallback)
    {
        if (entries.Count != 0)
        {
            Console.WriteLine($"Select the entry to {action}:");
            for (var i = 0; i < entries.Count; i++)
                Console.WriteLine($"{i + 1}. {entries[i].Domain} : {entries[i].ProxyType} : {entries[i].ProxyDetails}");
            Console.Write($"Enter the number of the entry to {action}: ");
            if (int.TryParse(Console.ReadLine(), out var selectedIndex) && selectedIndex >= 1 &&
                selectedIndex <= entries.Count)
                actionCallback(entries, selectedIndex - 1);
            else
                Console.WriteLine("Invalid entry number.");
        }
        else
        {
            Console.WriteLine("No entries found.");
        }
    }

    public static void ListAllEntries()
    {
        var entries = ReadEntriesFromFile();

        // Separate entries into file server and reverse proxy lists
        var fileServerEntries = entries.Where(entry => entry.ProxyType == "file_server").ToList();
        var reverseProxyEntries = entries.Where(entry => entry.ProxyType != "file_server").ToList();

        Console.ForegroundColor = ConsoleColor.Cyan; // Set color to yellow for file server category
        Console.WriteLine("Static File Server Entries:");
        Console.ResetColor(); // Reset color

        foreach (var entry in fileServerEntries)
            Console.WriteLine($"{entry.Domain} : Static File Server : {entry.ProxyDetails}");

        Console.ForegroundColor = ConsoleColor.Cyan; // Set color to cyan for reverse proxy category
        Console.WriteLine("\nReverse Proxy Entries:");
        Console.ResetColor(); // Reset color

        foreach (var entry in reverseProxyEntries)
            Console.WriteLine($"{entry.Domain} : Reverse Proxy : {entry.ProxyDetails}");
    }


    public static void ValidateCaddyfile()
    {
        try
        {
            var process = new System.Diagnostics.Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "proxydata",
                        "caddy"), // Specify the path to the caddy executable
                    Arguments =
                        $"fmt \"{Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "proxydata", "Caddyfile")}\" -w", // Adjust the path to Caddyfile
                    WorkingDirectory =
                        AppDomain.CurrentDomain
                            .BaseDirectory, // Set the working directory to the application's base directory
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };

            process.Start();
            process.WaitForExit();

            process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();

            Console.WriteLine(string.IsNullOrWhiteSpace(error)
                ? "Caddyfile is valid and formatted successfully."
                : $"Error validating and formatting Caddyfile: {error}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error validating and formatting Caddyfile: {ex.Message}");
        }
    }
}

public class Entry
{
    public string? Domain { get; set; }
    public string? ProxyType { get; set; } // Indicates whether it's a reverse proxy or file server

    public string?
        ProxyDetails { get; set; } // Details like IP address and port for reverse proxy or root path for file server
}