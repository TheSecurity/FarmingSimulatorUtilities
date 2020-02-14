namespace FarmingSimulatorUtilities.ConsoleApp.Entities
{
    public class Credentials
    {
        public Credentials(string username)
        {
            Username = username;
        }

        public string Username { get; set; }
    }
}