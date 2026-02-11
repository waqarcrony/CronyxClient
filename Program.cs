using Cronyx;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Cronyx.Services;

IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService() // Allows running as a Windows Service
    .ConfigureServices((context, services) =>
    {
        services.AddHostedService<Worker>(); // Register the worker
        services.AddSingleton<ICronyxService, CronyxService>(); // DI for your service
    })
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();
        ILoggingBuilder loggingBuilder = logging.AddEventLog(); // Log to Windows Event Log
    })
    .Build();

await host.RunAsync();
