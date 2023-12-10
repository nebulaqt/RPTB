using RPTB.Utilities;

internal class Program
{
    private static void Main()
    {
        do
        {
            Console.Clear(); // Clear the console only once
            DisplayAsciiArt("RPTB");

            Console.WriteLine("Select an option:");
            Console.WriteLine("1. Create Config");
            Console.WriteLine("2. Add Reverse Proxy Entry");
            Console.WriteLine("3. Start");
            Console.WriteLine("4. Stop");
            Console.WriteLine("5. Restart");
            Console.WriteLine("6. Exit");

            Console.Write("Enter Option: ");
            string userInput = Console.ReadLine();

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
                    Console.WriteLine("Starting...");
                    break;
                case "4":
                    Console.WriteLine("Stopping...");
                    break;
                case "5":
                    Console.WriteLine("Restarting...");
                    break;
                case "6":
                    Console.WriteLine("Exiting...");
                    Environment.Exit(0);
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please enter a valid option.");
                    break;
            }

            Thread.Sleep(1000); // Optional delay
        } while (true);
    }

    private static void DisplayAsciiArt(string text)
    {
        Console.WriteLine(@"
_________________________________________ 
\______   \______   \__    ___/\______   \
 |       _/|     ___/ |    |    |    |  _/
 |    |   \|    |     |    |    |    |   \
 |____|_  /|____|     |____|    |______  /
        \/                             \/ 
");
    }
}
