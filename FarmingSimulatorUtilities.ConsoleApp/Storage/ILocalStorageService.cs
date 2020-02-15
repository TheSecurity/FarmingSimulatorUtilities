using System.IO;

namespace FarmingSimulatorUtilities.ConsoleApp.Storage
{
    public interface ILocalStorageService
    {
        bool TryInsertConfigurationPath(string path, out string errorMessage);
        void InsertCredentials(string username);
        bool TryGetConfigurationPath(out string path);
        bool TryGetUsername(out string username);
        void WriteFile(ref MemoryStream stream, string path);
        void DeleteFile(string path);
        string GetLockfileContent(ref MemoryStream stream);
        void DeletePreviousSave();
    }
}