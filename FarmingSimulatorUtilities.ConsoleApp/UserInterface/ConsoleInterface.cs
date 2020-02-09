using System;
using System.Drawing;
using System.Threading;
using FarmingSimulatorUtilities.ConsoleApp.Storage;

namespace FarmingSimulatorUtilities.ConsoleApp.UserInterface
{
    public class ConsoleInterface
    {
        private readonly ILocalStorageService _localStorage;

        public ConsoleInterface(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public void InitializeInterface()
        {
            DrawUserInterface();
            PrintMenu();
            Run();
            Console.ReadKey();
        }
       
        public void DrawUserInterface()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(
                @"  ______ _____ ___   ___  __  ___    _    _ _   _ _ _ _   _           " + "\n" +
                @" |  ____/ ____|__ \ / _ \/_ |/ _ \  | |  | | | (_) (_) | (_)          " + "\n" +
                @" | |__ | (___    ) | | | || | (_) | | |  | | |_ _| |_| |_ _  ___  ___ " + "\n" +
                @" |  __| \___ \  / /| | | || |\__, | | |  | | __| | | | __| |/ _ \/ __|" + "\n" +
                @" | |    ____) |/ /_| |_| || |  / /  | |__| | |_| | | | |_| |  __/\__ \" + "\n" +
                @" |_|   |_____/|____|\___/ |_| /_/    \____/ \__|_|_|_|\__|_|\___||___/" + "\n" +
                @"                                                                      " + "\n" +
                "======================================================================\n\n\n");
        }

        private static void PrintMenu()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Type:" +
                              "\t 1 : Load file\n" +
                              "\t 2 : Save file\n" +
                              "\t 3 : Insert save folder path\n" +
                              "\t 4 : Insert credentials\n\n" +
                              "\t Type exit for leaving the program.\n\n");
            Console.Write("Your choice: ");
            Console.ForegroundColor = ConsoleColor.Magenta;
        }

        private void Run()
        {
            var input = GetStandardizedConsoleInput();

            while(input != "exit")
            {
                switch (input)
                {
                    case "1": LoadFile();
                        break;
                    case "2": SaveFile();
                        break;
                    case "3": InsertSavePath();
                        break;
                    case "4": InsertCredentials();
                        break;
                }

                DrawUserInterface();
                PrintMenu();

                input = GetStandardizedConsoleInput();
            }

        }

        private static string GetStandardizedConsoleInput()
            => Console.ReadLine().ToLower().Replace(" ", "");

        private void LoadFile()
        {

        }

        private void SaveFile()
        {

        }

        private void DrawSection(string sectionName, string instructions)
        {
            DrawUserInterface();

            Console.WriteLine($"{sectionName}\n\n");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(instructions + "\n");
        }

        private void InsertSavePath()
        {
            DrawSection("Insert Save Path Section", "Insert absolute path to root folder of your save. And press enter");

            SendSystemMessage("Path: ");

            var path = Console.ReadLine();

            SendSystemMessage($"\nYou entered this path: \"{path}\". Make sure it is valid. Type yes if it is valid. Otherwise type no.\n\nYour choice: ");

            if (GetStandardizedConsoleInput() != "yes")
            {
                SendNotificationMessage("Resetting process of adding a save path.", ConsoleColor.Red, 2000);
                InsertSavePath();
                return;
            }

            if (_localStorage.TryInsertConfigurationPath(path, out var errorMessage))
            {
                SendNotificationMessage("Success!", ConsoleColor.Green, 1500);

                DrawUserInterface();
                PrintMenu();
                return;
            }

            SendNotificationMessage(errorMessage + "\nPath was not saved.", ConsoleColor.Red, 2000);
        }

        private void InsertCredentials()
        {
            DrawSection("Insert Credentials Section", "Insert your credentials like: email:password. Example: admin@gmail.com:MyVeryStrongPassword. Don't use : in your password");

            SendSystemMessage("Credentials: ");


            var path = Console.ReadLine();
            var content = path.Split(':');

            if (content.Length != 2)
            {
                SendNotificationMessage("Error! Wrong credentials inserted. Resetting process of inserting credentials.", ConsoleColor.Red, 3000);
                InsertCredentials();
                return;
            }

            SendSystemMessage($"\nYou entered username: \"{content[0]}\" and password: \"{content[1]}\". Make sure it is valid. Type yes if it is valid. Otherwise type no.\n\nYour choice: ");

            if (GetStandardizedConsoleInput() != "yes")
            {
                SendNotificationMessage("Resetting process of inserting credentials.", ConsoleColor.Red, 2000);
                InsertCredentials();
                return;
            }

            if (_localStorage.TryInsertCredentials(content[0], content[1], out var errorMessage))
            {
                SendNotificationMessage("Success!", ConsoleColor.Green, 1500);

                DrawUserInterface();
                PrintMenu();
                return;
            }

            SendNotificationMessage(errorMessage + "\nPath was not saved.", ConsoleColor.Red, 2000);
        }

        private static void SendNotificationMessage(string message, ConsoleColor color, int millisecondsTimeout)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Thread.Sleep(millisecondsTimeout);
        }

        private static void SendSystemMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(message); 
            Console.ForegroundColor = ConsoleColor.Magenta;
        }
    }
}
