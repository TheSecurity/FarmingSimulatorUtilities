using System.IO;

namespace FarmingSimulatorUtilities.ConsoleApp.Storage
{
    public interface ILocalStorageService
    {
        bool TryInsertConfigurationPath(string path, out string errorMessage);
        void InsertCredentials(string username);
        bool TryGetConfigurationPath(out string path);
        void WriteFile(MemoryStream stream, string path);
        void DeleteFile(string path);

    }
}