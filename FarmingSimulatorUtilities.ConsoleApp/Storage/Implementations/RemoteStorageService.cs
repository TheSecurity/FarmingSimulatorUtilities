using System;
using System.IO;
using System.Threading;
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

    }
}