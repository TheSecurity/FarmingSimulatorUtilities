using System.IO;

namespace FarmingSimulatorUtilities.ConsoleApp.Storage
{
    public interface IRemoteStorageService
    {
        void UploadFile(string archivePath);
        MemoryStream DownloadFile(out string? fileName);
    }
}