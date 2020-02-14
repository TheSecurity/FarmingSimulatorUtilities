using System.IO;
using FarmingSimulatorUtilities.ConsoleApp.Entities;
using Newtonsoft.Json;

namespace FarmingSimulatorUtilities.ConsoleApp.Storage.Implementations
{
    public class LocalStorageService : ILocalStorageService
    {
        private const string ConfigurationFilePath = @"Resources\config.json";
        private const string CredentialsFilePath = @"Resources\credentials.json";

        public bool CredentialsFileExists()
            => File.Exists(CredentialsFilePath);

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

        public void WriteFile(MemoryStream stream, string path)
        {
            using var file = new FileStream(path, FileMode.Create, FileAccess.Write);
            stream.WriteTo(file);
        }

        public void DeleteFile(string path) 
            => File.Delete(path);
    }
}