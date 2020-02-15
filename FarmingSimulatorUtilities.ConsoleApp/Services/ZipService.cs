using System;
using FarmingSimulatorUtilities.ConsoleApp.Extensions;

namespace FarmingSimulatorUtilities.ConsoleApp.Services
{
    public class ZipService
    {
        public string ZipFile(string path)
        {
            var archivePath = $@"Resources\save_{DateTime.Now.ToDateTimeString()}.zip";
            System.IO.Compression.ZipFile.CreateFromDirectory(path, archivePath);
            return archivePath;
        }

        public void UnZipFile(string sourcePath, string destinationPath)
        {
            System.IO.Compression.ZipFile.ExtractToDirectory(sourcePath, destinationPath);
        }
    }
}