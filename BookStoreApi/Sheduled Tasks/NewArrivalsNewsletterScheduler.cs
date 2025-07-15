using BookStoreApi;
using Microsoft.Extensions.DependencyInjection; // Required for CreateScope()
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BookStoreApi.Sheduled_Tasks // Adjust namespace
{
    public class NewArrivalsNewsletterScheduler : BackgroundService
    {
        private readonly ILogger<NewArrivalsNewsletterScheduler> _logger;
        private readonly IServiceProvider _serviceProvider; // Used to create a scope for services

        // Constructor to inject services like ILogger and IServiceProvider
        public NewArrivalsNewsletterScheduler(
            ILogger<NewArrivalsNewsletterScheduler> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("New Arrivals Newsletter Scheduler is starting.");

            // For testing, let's make it run every 30 seconds initially.
            // For a real daily task, you'd calculate time until next specific hour (e.g., 2 AM)
            int runIntervalSeconds = 30; // Shorter for testing purposes
            _logger.LogInformation("Next run in {Seconds} seconds.", runIntervalSeconds);

            while (!stoppingToken.IsCancellationRequested) // Loop until cancellation is requested
            {
                try
                {
                    // Wait for the specified interval, respecting cancellation
                    await Task.Delay(TimeSpan.FromSeconds(runIntervalSeconds), stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    // This exception is expected if the token is cancelled while awaiting Task.Delay
                    _logger.LogInformation("New Arrivals Newsletter Scheduler: Task.Delay was cancelled (app shutting down).");
                    break; // Exit the loop gracefully
                }

                _logger.LogInformation("New Arrivals Newsletter Scheduler is executing at: {time}", DateTimeOffset.Now);

                try
                {
                    // Create a new dependency injection scope for the task's execution.
                    // This is crucial because services like DbContext are often scoped,
                    // and a background service runs as a singleton.
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var newArrivalsProcessor = scope.ServiceProvider.GetRequiredService<INewArrivalsProcessor>();
                        await newArrivalsProcessor.ProcessNewArrivalsAsync();
                    }
                }
                catch (OperationCanceledException)
                {
                    // This exception is also expected if the processing itself respects cancellation
                    _logger.LogInformation("New Arrivals Newsletter processing was cancelled gracefully.");
                    break; // Exit the loop
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while processing new arrivals.");
                }
            }

            _logger.LogInformation("New Arrivals Newsletter Scheduler has stopped gracefully.");
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("New Arrivals Newsletter Scheduler is performing final cleanup...");
            await base.StopAsync(stoppingToken); // Call base implementation
            _logger.LogInformation("New Arrivals Newsletter Scheduler has shut down completely.");
        }
    }
}