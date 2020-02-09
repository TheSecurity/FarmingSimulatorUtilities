namespace FarmingSimulatorUtilities.ConsoleApp.Storage
{
    public interface ILocalStorageService
    {
        bool TryInsertConfigurationPath(string path, out string errorMessage);
        bool TryInsertCredentials(string username, string password, out string errorMessage);
        bool TryGetConfigurationPath(out string path);
    }
}