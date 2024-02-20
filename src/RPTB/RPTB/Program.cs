﻿using System;
using System.Collections.Generic;
using RPTB.Config;
using RPTB.ProcUtilities;
using RPTB.Remote;
using RPTB.Utilities;

namespace RPTB
{
    internal static class Program
    {
        private static bool isMonitoring = false;

        private const string Logo = @"
    _________________________________________
    \______   \______   \__    ___/\______   \
     |       _/|     ___/ |    |    |    |  _/
     |    |   \|    |     |    |    |    |   \
     |____|_  /|____|     |____|    |______  /
            \/                             \/   

        ";

        private static readonly List<string> MenuOptions = new()
        {
            "Add Entry",
            "Update Entry",
            "Delete Entry",
            "List Entries",
            "Validate Caddyfile",
            "Start Caddy Process",
            "Stop Caddy Process",
            "Restart Caddy Process",
            "Update Caddy",
            "Check Domain",
            "Toggle Process Monitor",
            "Exit"
        };

        private static void Main()
        {
            Initialize();
            RunMainMenuLoop();
        }

        private static void Initialize()
        {
            Console.WriteLine("Selected Operating System: Windows");
        }

        private static void RunMainMenuLoop()
        {
            while (true)
            {
                DisplayMainMenu(isMonitoring);

                var userInput = Console.ReadLine();
                if (int.TryParse(userInput, out var option) && option > 0 && option <= MenuOptions.Count)
                {
                    if (option == 11) // If the option is to toggle monitoring, update the status
                    {
                        isMonitoring = !isMonitoring;
                    }
                    ExecuteOption(option);
                }
                else
                {
                    Console.WriteLine("Invalid choice. Please enter a valid option.");
                }
            }
        }

        private static void DisplayMainMenu(bool isMonitoring)
        {
            Console.Clear();
            string centeredLogo = PrintCentered(Logo);
            Console.WriteLine(centeredLogo);
            Console.WriteLine(SystemInfo.GetOperatingSystemInfo());
            Console.WriteLine(SystemInfo.GetRuntimeInfo());
            Console.WriteLine($"Monitoring: {(isMonitoring ? "True" : "False")}");
            Console.WriteLine();

            DisplayMenuOptions(MenuOptions);
        }

        private static string PrintCentered(string text)
        {
            int width = Console.WindowWidth;
            string[] lines = text.Split('\n');
            string centeredText = "";
            foreach (string line in lines)
            {
                int leftPadding = (width - line.Length) / 2;
                centeredText += line.PadLeft(leftPadding + line.Length) + "\n";
            }
            return centeredText;
        }

        private static void ExecuteOption(int option)
        {
            Console.Clear();

            switch (option)
            {
                case 1:
                    ConfigManager.AddEntry();
                    break;
                case 2:
                    ConfigManager.UpdateEntry();
                    break;
                case 3:
                    ConfigManager.DeleteEntry();
                    break;
                case 4:
                    ConfigManager.ListAllEntries();
                    break;
                case 5:
                    ConfigManager.ValidateCaddyfile();
                    break;
                case 6:
                    ProcessManager.StartCaddyProcess();
                    break;
                case 7:
                    ProcessManager.StopCaddyProcess();
                    break;
                case 8:
                    ProcessManager.RestartCaddyProcess();
                    break;
                case 9:
                    _ = Updater.DownloadCaddyPortableAsync();
                    break;
                case 10:
                    AskUserForDomain();
                    break;
                case 11:
                    if (isMonitoring)
                    {
                        ProcessMonitor.StopMonitoring();
                        isMonitoring = false;
                        Console.WriteLine("Monitoring process stopped.");
                    }
                    else
                    {
                        ProcessMonitor.StartMonitoring();
                        isMonitoring = true;
                        Console.WriteLine("Monitoring process started.");
                    }
                    break;
                case 12:
                    ExitProgram();
                    break;
                default:
                    Console.WriteLine("Invalid option. Please enter a valid option.");
                    break;
            }
            Console.ReadLine(); // Pause after executing the action
        }

        private static void AskUserForDomain()
        {
            Console.WriteLine("Enter domain: ");
            var domain = Console.ReadLine();
            if (domain != null) _ = CheckDomain.EvaluateDomainStatusAsync(domain);
            Console.ReadKey();
        }

        private static void ExitProgram()
        {
            Console.WriteLine("Exiting...");
            Environment.Exit(0);
        }

        private static void DisplayMenuOptions(IEnumerable<string> options)
        {
            var index = 1;
            foreach (var option in options)
            {
                Console.WriteLine($"{index}. {option}");
                index++;
            }
            Console.Write("Enter Option: ");
        }
    }
}
