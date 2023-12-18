using RPTB.Config;
using RPTB.Process;
using RPTB.Remote;
using RPTB.Utilities;

namespace RPTB;

internal static class Program
{
    // Load configuration
    private static readonly AppConfig? AppConfig = AppConfig.Load("appconfig.json");

    private static readonly Dictionary<string, Action> Actions = new()
    {
        { "1", ConfigManager.CreateConfig },
        { "2", /*ConfigManager.AddStaticFileServer*/ () => Console.WriteLine("Not implemented") },
        { "3", ConfigManager.AddEntry },
        { "4", ConfigManager.UpdateEntry },
        { "5", ConfigManager.DeleteEntry },
        { "6", () => RunWithPause(ConfigManager.ListEntries) },
        { "7", () => RunWithPause(ConfigManager.ValidateCaddyfile) },
        { "8", () => RunWithPause(() => ProcessManager.StartCaddyProcess(_selectedOperatingSystem)) },
        { "9", ProcessManager.StopCaddyProcess },
        { "10", () => RunWithPause(() => ProcessManager.RestartCaddyProcess(_selectedOperatingSystem)) },
        { "11", () => RunWithPause(() => Updater.DownloadCaddyPortableAsync(_selectedOperatingSystem).Wait()) },
        { "12", AskUserForDomain },
        { "13", () => RunWithPause(() => ProcessMonitor.MonitorCaddyProcess(_selectedOperatingSystem)) },
        { "14", ExitProgram }
    };


    private static readonly List<string> MenuOptions =
    [
        "Initial Configuration",
        "Add Static File Server",
        "Add Reverse Proxy Entry",
        "Edit Reverse Proxy Entry",
        "Delete Reverse Proxy Entry",
        "List Entries",
        "Validate AppConfig",
        "Start",
        "Stop",
        "Restart",
        "Update Caddy",
        "Check Domain",
        "Monitor Process",
        "Exit"
    ];

    private static string? _selectedOperatingSystem;

    private static void Main()
    {
        if (AppConfig?.SelectedOperatingSystem == null)
            // If the operating system is not set in the configuration, prompt the user to select it
            SelectOperatingSystem();
        else
            // Use the operating system from the configuration
            _selectedOperatingSystem = AppConfig.SelectedOperatingSystem;

        var exitRequested = false;

        do
        {
            Console.Clear();
            DisplayAsciiArt();
            Console.WriteLine(SystemInfo.GetOperatingSystemInfo());
            Console.WriteLine(SystemInfo.GetRuntimeInfo());
            Console.WriteLine($"Selected Operating System: {_selectedOperatingSystem}");
            Console.WriteLine();
            DisplayMenuOptions(MenuOptions);

            var userInput = Console.ReadLine();
            if (userInput != null && Actions.TryGetValue(userInput, out var action))
            {
                if (action == ExitProgram)
                {
                    exitRequested = true;
                }
                else
                {
                    action.Invoke();
                    Console.ReadLine(); // Pause after executing the action
                }
            }
            else
            {
                Console.WriteLine("Invalid choice. Please enter a valid option.");
            }
        } while (!exitRequested);
    }

    private static void SelectOperatingSystem()
    {
        Console.WriteLine("Select the operating system:");
        Console.WriteLine("Enter 'w' for Windows, 'l' for Linux, 'm' for macOS:");
        _selectedOperatingSystem = Console.ReadLine()?.Trim().ToLower();

        switch (_selectedOperatingSystem)
        {
            case "w":
            case "l":
            case "m":
                break;
            default:
                Console.WriteLine("Unsupported operating system. Defaulting to Windows.");
                _selectedOperatingSystem = "w"; // Default to Windows
                break;
        }

        // Save the selected operating system in the configuration
        if (AppConfig == null) return;
        AppConfig.SelectedOperatingSystem = _selectedOperatingSystem;
        AppConfig.Save("appconfig.json");
    }

    private static void RunWithPause(Action action)
    {
        action.Invoke();
        Console.ReadLine();
    }

    private static void AskUserForDomain()
    {
        Console.WriteLine("Checking Domain...");
        Console.WriteLine("Enter domain: ");
        var domain = Console.ReadLine();
        if (domain != null) CheckDomain.EvaluateDomainStatusAsync(domain).Wait();
        Console.ReadKey();
    }

    private static void ExitProgram()
    {
        Console.WriteLine("Exiting...");
        Environment.Exit(0);
    }

    private static void DisplayMenuOptions(IReadOnlyList<string> options)
    {
        for (var i = 0; i < options.Count; i++) Console.WriteLine($"{i + 1}. {options[i]}");
        Console.Write("Enter Option: ");
    }

    private static void DisplayAsciiArt()
    {
        const string logo = @"
    _________________________________________
    \______   \______   \__    ___/\______   \
     |       _/|     ___/ |    |    |    |  _/
     |    |   \|    |     |    |    |    |   \
     |____|_  /|____|     |____|    |______  /
            \/                             \/                                                                                                                                       
        ";

        var logoLines = logo.Split('\n');
        var consoleWidth = Console.WindowWidth;

        // Find the length of the longest line in the ASCII art
        var maxLength = logoLines.Max(line => line.Length);

        foreach (var line in logoLines)
        {
            var padding = Math.Max((consoleWidth - maxLength) / 2, 0);
            Console.WriteLine(new string(' ', padding) + line);
        }
    }
}