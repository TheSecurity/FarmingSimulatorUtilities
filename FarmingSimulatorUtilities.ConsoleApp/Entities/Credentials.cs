namespace FarmingSimulatorUtilities.ConsoleApp.Entities
{
    public class Credentials
    {
        public Credentials(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public string Username { get; set; }
        public string Password { get; set; }

    }
}