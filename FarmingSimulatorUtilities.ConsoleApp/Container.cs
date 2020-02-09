using System;
using FarmingSimulatorUtilities.ConsoleApp.Services;
using FarmingSimulatorUtilities.ConsoleApp.UserInterface;
using Microsoft.Extensions.DependencyInjection;

namespace FarmingSimulatorUtilities.ConsoleApp
{
    public static class Container
    {
        public static IServiceProvider Services ()
        => new ServiceCollection()
            .AddSingleton<ConsoleInterface>()
            .AddSingleton<StorageService>()
            .AddSingleton<ZipService>()
            .BuildServiceProvider();
    }
}