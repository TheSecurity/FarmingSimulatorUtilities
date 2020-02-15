using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using File = Google.Apis.Drive.v3.Data.File;

namespace FarmingSimulatorUtilities.ConsoleApp.Storage.Repositories.Implementations
{
    public class RemoteStorageRepository : IRemoteStorageRepository
    {
        private static DriveService _driveService;
        private static readonly string[] Scopes = { DriveService.Scope.Drive };
        private const string ApplicationName = "FarmingSimulatorUtilities";

        public RemoteStorageRepository()
        {
            UserCredential credential;

            using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                const string credentialsPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credentialsPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credentialsPath);
            }

            // Create Drive API service.
            _driveService = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
        }

        public void UploadFile(Stream stream, string name, string mimeType)
        {
            var fileMetadata = new File { Name = name, MimeType = mimeType };

            var request = _driveService.Files.Create(fileMetadata, stream, mimeType);
            request.Fields = "id";
            request.Upload();

            var file = request.ResponseBody;
        }

        public List<File> GetAllFiles()
        {
            // Define parameters of request.
            var listRequest = _driveService.Files.List();
            listRequest.PageSize = 999;
            listRequest.Fields = "nextPageToken, files(id, name)";

            var request = listRequest.Execute();

            return request.Files.ToList();
        }

        public MemoryStream DownloadFile(File file)
        {
            var request = _driveService.Files.Get(file.Id);
            var stream = new MemoryStream();

            request.MediaDownloader.ProgressChanged += progress =>
            {
                switch (progress.Status)
                {
                    case Google.Apis.Download.DownloadStatus.Downloading:
                    {
                        Console.WriteLine(progress.BytesDownloaded);
                        break;
                    }
                    case Google.Apis.Download.DownloadStatus.Completed:
                    {
                        Console.WriteLine("Download complete.");
                        break;
                    }
                    case Google.Apis.Download.DownloadStatus.Failed:
                    {
                        Console.WriteLine("Download failed.");
                        break;
                    }
                }
            };

            request.Download(stream);
            return stream;
        }

        public void DeleteFile(string fileId)
        {
            var deleteRequest = _driveService.Files.Delete(fileId);
            deleteRequest.Execute();

            _driveService.Files.EmptyTrash().Execute();
        }
    }
}