using System;
using System.IO;
using System.Threading;
using FarmingSimulatorUtilities.ConsoleApp.Services;
using FarmingSimulatorUtilities.ConsoleApp.Storage;

namespace FarmingSimulatorUtilities.ConsoleApp.UserInterface
{
    public class ConsoleInterface
    {
        private readonly ILocalStorageService _localStorage;
        private readonly IRemoteStorageService _remoteStorage;
        private readonly ZipService _zipService;

        public ConsoleInterface(ILocalStorageService localStorage, IRemoteStorageService remoteStorage, ZipService zipService)
        {
            _localStorage = localStorage;
            _remoteStorage = remoteStorage;
            _zipService = zipService;
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
            DrawSection("Load File Section", "File loading is in progress.");

            if (!_localStorage.TryGetConfigurationPath(out var path))
            {
                SendErrorMessage("Save path was not set!", 2000);
                return;
            }

            var stream = _remoteStorage.DownloadFile(out var fileName);

            if (fileName is null)
            {
                SendErrorMessage("File not found!", 2000);
            }

            var zipFilePath = $"{path}/{fileName}";

            _localStorage.WriteFile(stream, zipFilePath);
            _zipService.UnZipFile(zipFilePath, path);
            _localStorage.DeleteFile(zipFilePath);
        }

        private void SaveFile()
        {
            DrawSection("Save File Section", "Save file is in progress.");

            if (!_localStorage.TryGetConfigurationPath(out var path))
            {
                SendErrorMessage("Save path was not set!", 2000);
                return;
            }

            var archivePath = _zipService.ZipFile(path);
            _remoteStorage.UploadFile(archivePath);
            File.Delete(archivePath);
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

            _localStorage.InsertCredentials(username);
            SendNotificationMessage("Success!", ConsoleColor.Green, 1500);

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
