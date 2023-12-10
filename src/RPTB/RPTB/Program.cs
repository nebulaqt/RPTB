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

do
{
    Console.Clear();
    DisplayAsciiArt();
    Console.WriteLine(SystemInfo.GetOperatingSystemInfo());
    Console.WriteLine(SystemInfo.GetRuntimeInfo());
    Console.WriteLine();
    DisplayMenuOptions(menuOptions);

    var userInput = Console.ReadLine();

    switch (userInput)
    {
        case "1":
            Console.WriteLine("Creating Config");
            ConfigManager.CreateConfig();
            break;
        case "2":
            Console.WriteLine("Adding Entry");
            ConfigManager.AddEntry();
            break;
        case "3":
            Console.WriteLine("Editing Entry");
            ConfigManager.UpdateEntry();
            break;
        case "4":
            Console.WriteLine("Deleting Entry");
            ConfigManager.DeleteEntry();
            break;
        case "5":
            Console.WriteLine("Listing Entries");
            ConfigManager.ListEntries();
            Console.ReadLine();
            break;
        case "6":
            Console.WriteLine("Validating Config");
            ConfigManager.ValidateCaddyfile();
            Console.ReadLine();
            break;
        case "7":
            Console.WriteLine("Starting...");
            ProcessManager.StartCaddyProcess();
            break;
        case "8":
            Console.WriteLine("Stopping...");
            ProcessManager.StopCaddyProcess();
            break;
        case "9":
            Console.WriteLine("Restarting...");
            ProcessManager.RestartCaddyProcess();
            break;
        case "10":
            Console.WriteLine("Downloading Caddy...");
            await Updater.DownloadCaddyPortableAsync();
            break;
        case "11":
            Console.WriteLine("Checking Domain...");
            Console.WriteLine("Enter domain: ");
            Task.Run(async () =>
            {
                var domain = Console.ReadLine();
                if (domain != null) await CheckDomain.CheckWebsiteStatus(domain);
            }).GetAwaiter().GetResult();
            Console.ReadKey();
            break;
        case "12":
            Console.WriteLine("Exiting...");
            Environment.Exit(0);
            return;
        default:
            Console.WriteLine("Invalid choice. Please enter a valid option.");
            break;
    }

    Thread.Sleep(1000);
} while (true);


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