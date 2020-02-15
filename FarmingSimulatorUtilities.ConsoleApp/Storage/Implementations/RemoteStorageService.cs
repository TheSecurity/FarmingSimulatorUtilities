using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FarmingSimulatorUtilities.ConsoleApp.Extensions;
using FarmingSimulatorUtilities.ConsoleApp.Storage.Repositories;
using GoogleFile = Google.Apis.Drive.v3.Data.File;

namespace FarmingSimulatorUtilities.ConsoleApp.Storage.Implementations
{
    public class RemoteStorageService : IRemoteStorageService
    {
        private readonly IRemoteStorageRepository _remoteStorageRepo;
        private const string LockFileName = "lockfile.txt";


        public RemoteStorageService(IRemoteStorageRepository remoteStorageRepo)
        {
            _remoteStorageRepo = remoteStorageRepo;
        }

        private static bool GetLockFile(ref MemoryStream ms, string username)
        {
            var tw = new StreamWriter(ms);
            tw.Write($"Lock file for user: {username}\n" +
                     $"Created at: {DateTime.Now.ToDateTimeString()}");
            tw.Flush();
            ms.Position = 0;

            return true;
        }

        public void UploadZipFile(string archivePath)
        {
            using var stream = new FileStream(archivePath, FileMode.Open);
            _remoteStorageRepo.UploadFile(stream, Path.GetFileName(archivePath), "application/zip");
        }

        public MemoryStream DownloadSave(out string fileName, string username)
        {
            var latestSave = GetLatestSave();

            fileName = latestSave?.Name;

            if (latestSave is null) return null;

            var str = new MemoryStream();

            if(!GetLockFile(ref str, username)) return null;

            _remoteStorageRepo.UploadFile(str, LockFileName, "text/plain");

            return _remoteStorageRepo.DownloadFile(latestSave);
        }

        public bool TryGetLockFile(out MemoryStream stream, out string fileId)
        {
            var lockfile = _remoteStorageRepo.GetAllFiles().FirstOrDefault(x => x.Name == LockFileName);

            if (lockfile is null)
            {
                stream = null;
                fileId = null;
                return false;
            }

            stream = _remoteStorageRepo.DownloadFile(lockfile);

            if (stream is { })
            {
                fileId = lockfile.Id;
                return true;
            }

            stream = null;
            fileId = null;
            return false;
        }

        public void DeleteLockFile(string fileId) 
            => _remoteStorageRepo.DeleteFile(fileId);

        private GoogleFile GetLatestSave()
        {
            var files = _remoteStorageRepo.GetAllFiles();

            if (files is { } && files.Count > 0)
            {
                var latestSaveName = GetLatestSaveName(files.Select(x => x.Name).ToList());

                return files.FirstOrDefault(x => x.Name.Contains(latestSaveName));
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
    }
}