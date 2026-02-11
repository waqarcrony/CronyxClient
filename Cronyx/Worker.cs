using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;

using System;
using System.Threading;
using System.Threading.Tasks;
using Cronyx.Services;

namespace Cronyx
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ICronyxService _cronyx;

        public Worker(ILogger<Worker> logger, ICronyxService cronyxService)
        {
            _logger = logger;
            _cronyx = cronyxService;

            // Force immediate log
            System.Diagnostics.EventLog.WriteEntry("Cronyx",
                "Worker constructor called at " + DateTime.Now,
                System.Diagnostics.EventLogEntryType.Information);
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation("===== Cronyx Worker Started at {Time} =====", DateTime.Now);
                _logger.LogInformation("CommonApplicationData: {path}",
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData));

                var retryPolicy = Policy
                    .Handle<Exception>()
                    .WaitAndRetryAsync(
                        3,
                        _ => TimeSpan.FromSeconds(10),
                        (ex, timespan, retryCount) =>
                        {
                            _logger.LogWarning(ex, "Retry {RetryCount} - DoWorkAsync failed", retryCount);
                        }
                    );

                while (!stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation("Starting work cycle at {Time}", DateTime.Now);

                    try
                    {
                        await retryPolicy.ExecuteAsync(async () =>
                        {
                            await _cronyx.DoWorkAsync();
                        });

                        _logger.LogInformation("Work cycle completed successfully");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Work cycle failed after all retries");
                    }

                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                }

                _logger.LogInformation("Cronyx Worker stopping gracefully");
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "FATAL: ExecuteAsync crashed");

                // Fallback logging
                System.Diagnostics.EventLog.WriteEntry("Cronyx",
                    $"Worker crashed: {ex.Message}\n{ex.StackTrace}",
                    System.Diagnostics.EventLogEntryType.Error);

                throw;
            }
        }
    }
}
