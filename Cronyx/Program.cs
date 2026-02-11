using Cronyx;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Cronyx.Services;
using System;

try
{
    IHost host = Host.CreateDefaultBuilder(args)
        .UseWindowsService()
        .ConfigureServices((context, services) =>
        {
            services.AddHostedService<Worker>();
            services.AddSingleton<ICronyxService, CronyxService>();
        })
        .ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddEventLog(options =>
            {
                options.SourceName = "Cronyx";
                options.LogName = "Application";
            });
            logging.SetMinimumLevel(LogLevel.Information);
        })
        .Build();

    var logger = host.Services.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Cronyx service starting at {Time}", DateTime.Now);

    await host.RunAsync();
}
catch (Exception ex)
{
    // This will write to Event Log even if logger fails
    System.Diagnostics.EventLog.WriteEntry("Cronyx",
        $"Fatal startup error: {ex.Message}\n{ex.StackTrace}",
        System.Diagnostics.EventLogEntryType.Error);
    throw;
}