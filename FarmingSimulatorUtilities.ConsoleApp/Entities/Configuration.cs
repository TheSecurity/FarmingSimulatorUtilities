namespace FarmingSimulatorUtilities.ConsoleApp.Entities
{
    public class Configuration
    {
        public Configuration(string path)
        {
            SavePath = path;
        }

        public string SavePath { get; set; }
    }
}