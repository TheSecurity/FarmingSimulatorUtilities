using FarmingSimulatorUtilities.ConsoleApp.UserInterface;
using Microsoft.Extensions.DependencyInjection;

namespace FarmingSimulatorUtilities.ConsoleApp
{
    internal class Program
    {
        private static void Main() 
            => Container.Services().GetRequiredService<ConsoleInterface>().InitializeInterface();
    }
}
