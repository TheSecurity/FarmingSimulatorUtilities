using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using FarmingSimulatorUtilities.ConsoleApp.Extensions;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using File = Google.Apis.Drive.v3.Data.File;

namespace FarmingSimulatorUtilities.ConsoleApp.Storage.Implementations
{
    public class RemoteStorageService : IRemoteStorageService
    {
        private static DriveService _driveService;
        private static readonly string[] Scopes = { DriveService.Scope.Drive };
        private const string ApplicationName = "FarmingSimulatorUtilities";

        public RemoteStorageService()
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

        public void UploadFile(string archivePath)
        {
            var fileMetadata = new File {Name = Path.GetFileName(archivePath), MimeType = "application/zip" };

            using var stream = new FileStream(archivePath, FileMode.Open);

            var request = _driveService.Files.Create(fileMetadata, stream, "application/zip");
            request.Fields = "id";
            request.Upload();

            var file = request.ResponseBody;
        }

        public MemoryStream DownloadFile(out string? fileName)
        {
            string pageToken = null;

            var latestSave = GetLatestSave(ref pageToken);

            fileName = latestSave?.Name;

            return latestSave is null 
                ? null 
                : DownloadFile(latestSave);
        }

        private static File GetLatestSave(ref string pageToken)
        {
            // Define parameters of request.
            var listRequest = _driveService.Files.List();
            listRequest.PageSize = 1;
            listRequest.Fields = "nextPageToken, files(id, name)";
            listRequest.PageToken = pageToken;
            listRequest.Q = "mimeType='application/zip'";

            // List files.
            var request = listRequest.Execute();

            if (request.Files is { } && request.Files.Count > 0)
            {
                var latestSaveName = GetLatestSaveName(request.Files.Select(x => x.Name).ToList());
                pageToken = request.NextPageToken;

                return request.Files.FirstOrDefault(x => x.Name.Contains(latestSaveName));
            }

            Console.WriteLine("No files found.");

            return null;
        }

        private static string GetLatestSaveName(IEnumerable<string> names)
        {
            var dateTimes = names.Where(x => x.StartsWith("save_")).Select(x => x.Replace("save_", "").Replace(".zip", ""));
            var dates = new List<DateTime>();

            foreach (var dateString in dateTimes)
            {
                if(dateString.ToDateTime(out var date))
                    dates.Add(date.Value!);
            }

            var latestDate = dates.OrderByDescending(x => x.Date).FirstOrDefault();

            return $"save_{latestDate.ToDateTimeString()}.zip";
        }

        private static MemoryStream DownloadFile(File file)
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
    }
}