using System.Text;
using RPTB.Utilities;

var menuOptions = new List<string>
{
    "Create Config",
    "Add Reverse Proxy Entry",
    "Edit Reverse Proxy Entry",
    "Delete Reverse Proxy Entry",
    "List Reverse Proxy Entries",
    "Validate Config",
    "Start",
    "Stop",
    "Restart",
    "Update Caddy",
    "Check Domain",
    "Exit"
};

var actions = new Dictionary<string, Action>
{
    { "1", ConfigManager.CreateConfig },
    { "2", ConfigManager.AddEntry },
    { "3", ConfigManager.UpdateEntry },
    { "4", ConfigManager.DeleteEntry },
    {
        "5", () =>
        {
            ConfigManager.ListEntries();
            Console.ReadLine();
        }
    },
    {
        "6", () =>
        {
            ConfigManager.ValidateCaddyfile();
            Console.ReadLine();
        }
    },
    { "7", ProcessManager.StartCaddyProcess },
    { "8", ProcessManager.StopCaddyProcess },
    { "9", ProcessManager.RestartCaddyProcess },
    { "10", () => { Updater.DownloadCaddyPortableAsync().Wait(); } },
    { "11", AskUserForDomain },
    {
        "12", () =>
        {
            Console.WriteLine("Exiting...");
            Environment.Exit(0);
        }
    }
};

do
{
    Console.Clear();
    DisplayAsciiArt();
    Console.WriteLine(SystemInfo.GetOperatingSystemInfo());
    Console.WriteLine(SystemInfo.GetRuntimeInfo());
    Console.WriteLine();
    DisplayMenuOptions(menuOptions);

    var userInput = Console.ReadLine();
    if (userInput != null && actions.TryGetValue(userInput, out var value))
        value.Invoke();
    else
        Console.WriteLine("Invalid choice. Please enter a valid option.");
} while (true);

void AskUserForDomain()
{
    Console.WriteLine("Checking Domain...");
    Console.WriteLine("Enter domain: ");
    var domain = Console.ReadLine();
    if (domain != null) CheckDomain.EvaluateDomainStatusAsync(domain).Wait();
    Console.ReadKey();
}

void DisplayMenuOptions(IReadOnlyList<string> options)
{
    for (var i = 0; i < options.Count; i++) Console.WriteLine($"{i + 1}. {options[i]}");
    Console.Write("Enter Option: ");
}

void DisplayAsciiArt()
{
    const string logo = """
                        _________________________________________
                        \______   \______   \__    ___/\______   \
                         |       _/|     ___/ |    |    |    |  _/
                         |    |   \|    |     |    |    |    |   \
                         |____|_  /|____|     |____|    |______  /
                                \/                             \/
                        """;
    var logoLines = logo.Split('\n');
    var consoleWidth = Console.WindowWidth;
    foreach (var line in logoLines)
        Console.WriteLine(
            new StringBuilder().Append("{0,").Append(consoleWidth / 2 + line.Length / 2).Append('}').ToString(), line);
}