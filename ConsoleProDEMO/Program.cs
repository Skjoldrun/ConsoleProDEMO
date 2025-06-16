using ConsoleProDEMO.Services;
using ConsoleProDEMO.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace ConsoleProDEMO
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            using var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
            };

            var appConfig = AppSettingsHelper.GetConfiguration();
            Log.Logger = LogInitializer.CreateLogger(appConfig);

            Log.Information($"{ThisAssembly.AssemblyName} start");
            var appHost = BuildHost(args, appConfig);

            // Call scoped service(s) here ...
            using var scope = appHost.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<ISomeService>();

            try
            {
                await service.Run();

                await WaitForExitAsync(cts.Token);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Exception: {ex.Message}");
            }

            Log.Information($"{ThisAssembly.AssemblyName}  stop");
            Log.CloseAndFlush();
        }

        /// <summary>
        /// Configures the host with registering the interfaces and class types.
        /// Sets the Serilog logger as logging provider for typed ILogger injections.
        /// </summary>
        /// <returns>configured host to access its services</returns>
        private static IHost BuildHost(string[] args, IConfiguration appConfig)
        {
            var host = Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureAppConfiguration((hostingContext, configBuilder) =>
                {
                    configBuilder.Sources.Clear();
                    configBuilder.AddConfiguration(appConfig);
                })
                .ConfigureServices((context, services) =>
                {
                    // Add DI registration here ...
                    services.AddTransient<ISomeService, SomeService>();

                    // Lib registration extensions here ...
                })
                .Build();

            return host;
        }

        /// <summary>
        /// Asynchronously waits for a cancellation request from the user (e.g., Ctrl+C).
        /// Keeps the application alive without blocking threads.
        /// </summary>
        /// <param name="cancellationToken">Token triggered by Console.CancelKeyPress.</param>
        /// <returns>A task that completes when cancellation is requested.</returns>
        private static async Task WaitForExitAsync(CancellationToken cancellationToken)
        {
            await Console.Out.WriteLineAsync("Press [Ctrl]+[C] to exit the application ...");

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(1000, cancellationToken);
                }
            }
            catch (TaskCanceledException)
            {
                Log.Information("Application cancellation requested – shutting down cleanly.");
            }
        }
    }
}