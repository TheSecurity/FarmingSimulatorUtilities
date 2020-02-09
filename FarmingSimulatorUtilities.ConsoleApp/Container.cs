using System;
using FarmingSimulatorUtilities.ConsoleApp.Services;
using FarmingSimulatorUtilities.ConsoleApp.Storage;
using FarmingSimulatorUtilities.ConsoleApp.Storage.Implementations;
using FarmingSimulatorUtilities.ConsoleApp.UserInterface;
using Microsoft.Extensions.DependencyInjection;

namespace FarmingSimulatorUtilities.ConsoleApp
{
    public static class Container
    {
        public static IServiceProvider Services()
        => new ServiceCollection()
            .AddSingleton<ConsoleInterface>()
            .AddSingleton<IRemoteStorageService, RemoteStorageService>()
            .AddSingleton<ILocalStorageService, LocalStorageService>()
            .AddSingleton<ZipService>()
            .BuildServiceProvider();
    }
}