using System.Collections.Generic;
using System.Globalization;
using System.IO;
using File = Google.Apis.Drive.v3.Data.File;

namespace FarmingSimulatorUtilities.ConsoleApp.Storage.Repositories
{
    public interface IRemoteStorageRepository
    {
        void UploadFile(Stream stream, string name, string mimeType);
        List<File> GetAllFiles();
        MemoryStream DownloadFile(File file);
        void DeleteFile(string fileId);
    }
}