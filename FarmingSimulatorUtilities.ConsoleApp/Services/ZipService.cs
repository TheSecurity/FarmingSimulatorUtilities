using System;

namespace FarmingSimulatorUtilities.ConsoleApp.Services
{
    public class ZipService
    {

        public string ZipFile(string path)
        {
            var archivePath = $@"Resources\save_{GetId()}.zip";
            System.IO.Compression.ZipFile.CreateFromDirectory(path, archivePath);
            return archivePath;
        }

        private static string GetId() 
            => DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");

        public void UnZipFile()
        {

        }
    }
}