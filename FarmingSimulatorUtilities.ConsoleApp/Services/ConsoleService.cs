using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using FarmingSimulatorUtilities.ConsoleApp.Storage;

namespace FarmingSimulatorUtilities.ConsoleApp.Services
{
    public class ConsoleService
    {
        private readonly ILocalStorageService _localStorage;
        private readonly IRemoteStorageService _remoteStorage;
        private readonly ZipService _zipService;

        public ConsoleService(ILocalStorageService localStorage, IRemoteStorageService remoteStorage, ZipService zipService)
        {
            _localStorage = localStorage;
            _remoteStorage = remoteStorage;
            _zipService = zipService;
        }

        public bool TryGetLockFile(out MemoryStream stream, out string fileId)
        {
            var result = _remoteStorage.TryGetLockFile(out var str, out var fId);

            if (result)
            {
                stream = str;
                fileId = fId;
                return true;
            }

            stream = null;
            fileId = null;
            return false;
        }

        public string GetLockFileContent(ref MemoryStream lockFileStream)
            => _localStorage.GetLockfileContent(ref lockFileStream);

        public bool TryDownloadFile(string username, string path, out string error)
        {
            var stream = _remoteStorage.DownloadSave(out var fileName, username);

            if (fileName is null)
            {
                error = "File not found!";
                return false;
            }

            var zipFilePath = $@"{Directory.GetParent(path).FullName}\{fileName}";

            _localStorage.WriteFile(ref stream, zipFilePath);
            _localStorage.DeletePreviousSave();
            _zipService.UnZipFile(zipFilePath, path);
            _localStorage.DeleteFile(zipFilePath);

            error = null;
            return true;
        }

        public bool CheckValidConfigurationAndCredentials(out string configurationPath, out string username, out string errorMessage)
        {
            configurationPath = null;
            username = null;

            if (!_localStorage.TryGetConfigurationPath(out var path))
            {
                errorMessage = "Save path was not set!";
                return false;
            }

            if (!_localStorage.TryGetUsername(out var credentialUsername))
            {
                errorMessage = "Credentials was not found!";
                return false;
            }

            configurationPath = path;
            username = credentialUsername;
            errorMessage = null;
            return true;
        }

        public void UploadFile(string path)
        {
            var archivePath = _zipService.ZipFile(path);
            _remoteStorage.UploadZipFile(archivePath);
            _localStorage.DeleteFile(archivePath);
        }

        public bool ValidateLockFile(ref MemoryStream lockFileStream, string username, string fileId, out string error)
        {
            var content = _localStorage.GetLockfileContent(ref lockFileStream);
            var lockfileUsername = content.Split("user: ")[1].Split("\n").First();

            if (username != lockfileUsername)
            {
                error = content;
                return false;
            }

            error = null;
            _remoteStorage.DeleteLockFile(fileId);
            return true;
        }

        public bool TryInsertConfigurationPath(string path, out string errorMessage)
        {
            var result = _localStorage.TryInsertConfigurationPath(path, out var errorMsg);

            if (result)
            {
                errorMessage = null;
                return true;
            }

            errorMessage = errorMsg;
            return false;
        }

        public void InsertCredentials(string username)
            => _localStorage.InsertCredentials(username);
    }
}