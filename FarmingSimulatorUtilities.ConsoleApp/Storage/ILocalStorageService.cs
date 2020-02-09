using System.Runtime.InteropServices.ComTypes;

namespace FarmingSimulatorUtilities.ConsoleApp.Storage
{
    public interface ILocalStorageService
    {
        bool ConfigurationFileExists();
        bool CredentialsFileExists();
        bool TryInsertConfigurationPath(string path, out string errorMessage);
        bool TryInsertCredentials(string username, string password, out string errorMessage);
    }
}