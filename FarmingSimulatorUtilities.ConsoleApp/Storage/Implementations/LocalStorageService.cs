using System.IO;
using FarmingSimulatorUtilities.ConsoleApp.Entities;
using FarmingSimulatorUtilities.ConsoleApp.Extensions;
using Newtonsoft.Json;

namespace FarmingSimulatorUtilities.ConsoleApp.Storage.Implementations
{
    public class LocalStorageService : ILocalStorageService
    {
        private const string ConfigurationFilePath = @"Resources\config.json";
        private const string CredentialsFilePath = @"Resources\credentials.json";

        public bool ConfigurationFileExists() 
            => File.Exists(ConfigurationFilePath);

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

        public bool TryInsertCredentials(string email, string password, out string errorMessage)
        {
            if(!email.IsEmail())
            {
                errorMessage = "Provided email is not an email address";
                return false;
            }

            var json = JsonConvert.SerializeObject(new Credentials(email, password));
            File.WriteAllText(CredentialsFilePath, json);

            errorMessage = null;
            return true;
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
    }
}