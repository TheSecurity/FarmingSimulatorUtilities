using System;
using System.Threading;
using FarmingSimulatorUtilities.ConsoleApp.Services;

namespace FarmingSimulatorUtilities.ConsoleApp.UserInterface
{
    public class ConsoleInterface
    {
        private readonly ConsoleService _consoleService;

        public ConsoleInterface(ConsoleService consoleService)
        {
            _consoleService = consoleService;
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
                    case "1": DownloadFile();
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

        private void DownloadFile()
        {
            DrawSection("Download File Section", "File download is in progress.");

            if (!_consoleService.CheckValidConfigurationAndCredentials(out var path, out var username, out var configError))
            {
                SendErrorMessage(configError, 2000);
                return;
            }

            SendSystemMessage("Looking for lockfile... ");

            if (_consoleService.TryGetLockFile(out var lockFileStream, out _))
            {
                var content = _consoleService.GetLockFileContent(ref lockFileStream);
                SendSystemMessage(content + "\n");

                SendNotificationMessage("Save was not downloaded. Contact user to upload the latest version.\n", ConsoleColor.Red, 10000);
                return;
            }

            SendSystemMessage("Nothing found.\n\nDownloading save... ");

            if (!_consoleService.TryDownloadFile(username, path, out var error))
            {
                SendErrorMessage(error, 2000);
                return;
            }

            SendSuccessMessage("Success!", 2000);
        }

        private void SaveFile()
        {
            DrawSection("Save File Section", "Save file is in progress.");

            if (!_consoleService.CheckValidConfigurationAndCredentials(out var path, out var username, out var configError))
            {
                SendErrorMessage(configError, 2000);
                return;
            }

            SendSystemMessage("Looking for lockfile... ");

            if (_consoleService.TryGetLockFile(out var lockFileStream, out var fileId))
            {
                if (!_consoleService.ValidateLockFile(ref lockFileStream, username, fileId, out var error))
                {
                    SendWrongUploadUserNotification(error);
                    return;
                }
            }
            else
            { 
                SendSystemMessage("Nothing found.");
            }

            SendSystemMessage("Uploading save...\n");

            _consoleService.UploadFile(path);

            SendSuccessMessage("Success!", 2000);
        }

        private void SendWrongUploadUserNotification(string content)
        {
            SendSystemMessage(content + "\n");

            SendErrorMessage("Save was not uploaded. Contact user to upload the latest version.\n", 10000);
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

            if (_consoleService.TryInsertConfigurationPath(path, out var errorMessage))
            {
                SendNotificationMessage("Success!", ConsoleColor.Green, 1500);

                DrawUserInterface();
                PrintMenu();
                return;
            }

            SendErrorMessage(errorMessage + "\nPath was not saved.", 2000);
        }

        private void InsertCredentials()
        {
            DrawSection("Insert Credentials Section", "Insert your username and press enter.");

            SendSystemMessage("Username: ");

            var username = Console.ReadLine();

            SendSystemMessage(
                $"\nYou entered username: \"{username}\". Make sure it is valid. Type yes if it is valid. Otherwise type no.\n\nYour choice: ");

            if (GetStandardizedConsoleInput() != "yes")
            {
                SendNotificationMessage("Resetting process of inserting credentials.", ConsoleColor.Red, 2000);
                InsertCredentials();
                return;
            }

            _consoleService.InsertCredentials(username);
            SendSuccessMessage("Success!", 1500);
        }

        private void SendSuccessMessage(string message, int millisecondsTimeout)
        {
            SendNotificationMessage(message, ConsoleColor.Green, millisecondsTimeout);

            DrawUserInterface();
            PrintMenu();
        }

        private void SendErrorMessage(string message, int millisecondsTimeout)
        {
            SendNotificationMessage(message, ConsoleColor.Red, millisecondsTimeout);

            DrawUserInterface();
            PrintMenu();
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
