using System.IO;
using System.Linq;
using FarmingSimulatorUtilities.ConsoleApp.Entities;
using Newtonsoft.Json;

namespace FarmingSimulatorUtilities.ConsoleApp.Storage.Implementations
{
    public class LocalStorageService : ILocalStorageService
    {
        private const string LockFilePath = @"Resources\lockfile.txt";
        private const string ConfigurationFilePath = @"Resources\config.json";
        private const string CredentialsFilePath = @"Resources\credentials.json";

        public bool TryInsertConfigurationPath(string path, out string errorMessage)
        {
            if (!Directory.Exists(path))
            {
                errorMessage = "Save directory not found.";
                return false;
            }

            InsertConfigurationPath(path);

            errorMessage = null;
            return true;
        }

        private static void InsertConfigurationPath(string path)
        {
            var json = JsonConvert.SerializeObject(new Configuration(path));
            File.WriteAllText(ConfigurationFilePath, json);
        }

        public void InsertCredentials(string username)
        {
            var json = JsonConvert.SerializeObject(new Credentials(username));
            File.WriteAllText(CredentialsFilePath, json);
        }

        public bool TryGetConfigurationPath(out string path)
        {
            var json = File.ReadAllText(ConfigurationFilePath);
            var configuration = JsonConvert.DeserializeObject<Configuration>(json);
            if (string.IsNullOrEmpty(configuration?.SavePath))
            {
                path = "";
                return false;
            }

            path = configuration.SavePath;
            return true;
        }

        public void WriteFile(ref MemoryStream stream, string path)
        {
            using var file = new FileStream(path, FileMode.Create, FileAccess.Write);
            stream.WriteTo(file);
        }

        public void DeleteFile(string path) 
            => File.Delete(path);

        public string GetLockfileContent(ref MemoryStream stream)
        {
            WriteFile(ref stream, LockFilePath);
            var content = File.ReadAllText(LockFilePath);
            DeleteFile(LockFilePath);

            return content;
        }

        public void DeletePreviousSave()
        {
            if (!TryGetConfigurationPath(out var path)) return;

            var contents = Directory.GetDirectories(path);

            foreach (var f in contents) 
                Directory.Delete(f, true);
        }

        public bool TryGetUsername(out string username)
        {
            var json = File.ReadAllText(CredentialsFilePath);
            var credentials = JsonConvert.DeserializeObject<Credentials>(json);
            if (string.IsNullOrEmpty(credentials?.Username))
            {
                username = "";
                return false;
            }

            username = credentials.Username;
            return true;
        }
    }
}