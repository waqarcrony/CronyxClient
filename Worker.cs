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
        }
      
     
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var retryPolicy = Policy
                    .Handle<Exception>()
                    .WaitAndRetryForeverAsync(
                        sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(10),
                        onRetry: (exception, timespan, context) =>
                        {
                            _logger.LogWarning(exception, "Retrying due to error...");
                        });
            while (!stoppingToken.IsCancellationRequested)
            {
                await retryPolicy.ExecuteAsync(async () =>
                {
                   
                    await _cronyx.DoWorkAsync(); // Make sure this method is async
                });

                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }
}
