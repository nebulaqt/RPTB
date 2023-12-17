﻿using System.Text;
using RPTB.Utilities;

namespace RPTB;

internal static class Program
{
    private static readonly Dictionary<string, Action> Actions = new()
    {
        { "1", ConfigManager.CreateConfig },
        { "2", ConfigManager.AddEntry },
        { "3", ConfigManager.UpdateEntry },
        { "4", ConfigManager.DeleteEntry },
        { "5", () => RunWithPause(ConfigManager.ListEntries) },
        { "6", () => RunWithPause(ConfigManager.ValidateCaddyfile) },
        { "7", ProcessManager.StartCaddyProcess },
        { "8", ProcessManager.StopCaddyProcess },
        { "9", ProcessManager.RestartCaddyProcess },
        { "10", () => RunWithPause(() => Updater.DownloadCaddyPortableAsync().Wait()) },
        { "11", AskUserForDomain },
        { "12", ExitProgram }
    };

    private static readonly List<string> MenuOptions = new()
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

    private static void Main()
    {
        var exitRequested = false;

        do
        {
            Console.Clear();
            DisplayAsciiArt();
            Console.WriteLine(SystemInfo.GetOperatingSystemInfo());
            Console.WriteLine(SystemInfo.GetRuntimeInfo());
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
                new StringBuilder().Append("{0,").Append(consoleWidth / 2 + line.Length / 2).Append('}').ToString(),
                line);
    }
}