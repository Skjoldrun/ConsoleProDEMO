using ConsoleProDEMO.Services;
using ConsoleProDEMO.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace ConsoleProDEMO
{
    internal class Program
    {
        private static bool _keepRunning = true;

        private static async Task Main(string[] args)
        {
            Console.CancelKeyPress += delegate (object? sender, ConsoleCancelEventArgs e)
            {
                e.Cancel = true;
                _keepRunning = false;
            };

            var appConfig = AppSettingsHelper.GetAppConfigBuilder().Build();
            Log.Logger = LogInitializer.CreateLogger(appConfig);

            Log.Information($"{ThisAssembly.AssemblyName} start");

            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddTransient<ISomeService, SomeService>();
                })
                .UseSerilog()
                .Build();

            var service = ActivatorUtilities.GetServiceOrCreateInstance<SomeService>(host.Services);

            try
            {
                await service.Run();

                await Console.Out.WriteLineAsync("Press [Ctrl]+[C] to exit the application ...");
                while (_keepRunning)
                    Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Exception: {ex.Message}");
            }

            Log.Information($"{ThisAssembly.AssemblyName} stop");
            Log.CloseAndFlush();
        }
    }
}