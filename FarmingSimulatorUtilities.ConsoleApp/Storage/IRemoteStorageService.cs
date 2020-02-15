using System.IO;

namespace FarmingSimulatorUtilities.ConsoleApp.Storage
{
    public interface IRemoteStorageService
    {
        void UploadZipFile(string archivePath);
        MemoryStream DownloadSave(out string fileName, string username);
        bool TryGetLockFile(out MemoryStream stream, out string fileId);
        void DeleteLockFile(string fileId);
    }
}