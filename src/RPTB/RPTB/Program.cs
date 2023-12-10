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
    "Exit"
};

do
{
    Console.Clear();
    DisplayAsciiArt("RPTB");
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
            break;
        case "8":
            Console.WriteLine("Stopping...");
            break;
        case "9":
            Console.WriteLine("Restarting...");
            break;
        case "10":
            Console.WriteLine("Downloading Caddy...");
            await Updater.DownloadCaddyPortableAsync();
            break;
        case "11":
            Console.WriteLine("Exiting...");
            Environment.Exit(0);
            return;
        default:
            Console.WriteLine("Invalid choice. Please enter a valid option.");
            break;
    }

    Thread.Sleep(1000);
} while (true);


void DisplayMenuOptions(List<string> options)
{
    for (var i = 0; i < options.Count; i++) Console.WriteLine($"{i + 1}. {options[i]}");

    Console.Write("Enter Option: ");
}

void DisplayAsciiArt(string text)
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

    foreach (var line in logoLines) Console.WriteLine("{0," + (consoleWidth / 2 + line.Length / 2) + "}", line);
}