using BookStoreApi.Configurations;
using BookStoreApi.Data;
using BookStoreApi.Entities;
using BookStoreApi.Models.DTOs;
using BookStoreApi.Services.Email;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BookStoreApi.Sheduled_Tasks
{
    public class RentalReminderService:BackgroundService
    {
        private readonly ILogger<RentalReminderService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly RentalReminderServiceSettings _settings;

        //private TimeSpan _checkInterval = TimeSpan.FromHours(24);
        //private int time = 1000;

        public RentalReminderService(ILogger<RentalReminderService> logger, IServiceProvider serviceProvider, IOptions<BackgroundServiceSettings> settings)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _settings = settings.Value.RentalReminderService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Rental Reminder Background Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Rental Reminder Background Service running at: {time}", DateTime.Now);

                try
                {
                    // Create a new scope for each execution to ensure services
                    // like DbContext are correctly scoped and disposed.
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>(); 
                        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

                       // ICollection<string> Emails = null;
                        var date = DateTime.Now;
                        var twoDaysFromNowStart = DateTime.Now.AddDays(2).Date; 
                        var twoDaysFromNowEnd = twoDaysFromNowStart.AddDays(1).AddTicks(-1); 

                        _logger.LogInformation("Checking for rentals due between {StartDate:yyyy-MM-dd} and {EndDate:yyyy-MM-dd}", twoDaysFromNowStart, twoDaysFromNowEnd);

                        var rentalsDueSoon = await context.Rentals
                            .Include(r => r.Book) 
                            .Include(r => r.User)
                            .Where(r => r.Status == RentalStatus.Accepted && 
                                        r.ActualReturnDate == null && 
                                        r.ExpectedReturnDate >= twoDaysFromNowStart &&
                                        r.ExpectedReturnDate <= twoDaysFromNowEnd&&
                                        r.LastReminderEmailSentDate == null)
                            .ToListAsync(stoppingToken); 


                        if (rentalsDueSoon.Any())
                        {
                            _logger.LogInformation("Found {Count} rentals due in ~2 days.", rentalsDueSoon.Count());
                            foreach (var rental in rentalsDueSoon)
                            {
                                ICollection<string> bccEmailsForThisReminder = new List<string>();

                                if (!string.IsNullOrEmpty(rental.CreatedBy))
                                {
                                    var creatorEmail = await context.Users
                                        .Where(u => u.Id == rental.CreatedBy) // Compare string User.Id with string CreatedBy
                                        .Select(u => u.Email)
                                        .FirstOrDefaultAsync(stoppingToken);

                                    if (!string.IsNullOrEmpty(creatorEmail))
                                    {
                                        bccEmailsForThisReminder.Add(creatorEmail);
                                        _logger.LogDebug("Adding creator email '{CreatorEmail}' to BCC for Rental ID {RentalId}.", creatorEmail, rental.Id);
                                    }
                                    else
                                    {
                                        _logger.LogWarning("Creator email not found for Rental ID {RentalId} (CreatedBy: '{CreatedBy}'), skipping BCC for this creator.", rental.Id, rental.CreatedBy);
                                    }
                                }
                                else
                                {
                                    _logger.LogDebug("Rental ID {RentalId} has no 'CreatedBy' value, skipping creator email for BCC.", rental.Id);
                                }

                                if (!string.IsNullOrEmpty(rental.ApprovedBy) ) // Avoid adding same email twice
                                {
                                    var approverEmail = await context.Users
                                        .Where(u => u.Id == rental.ApprovedBy) // Compare string User.Id with string ApprovedBy
                                        .Select(u => u.Email)
                                        .FirstOrDefaultAsync(stoppingToken);

                                    Console.WriteLine(approverEmail);
                                   // Emails.Add(approverEmail);
                                    if (!string.IsNullOrEmpty(approverEmail))
                                    {
                                        bccEmailsForThisReminder.Add(approverEmail);
                                        _logger.LogDebug("Adding approver email '{ApproverEmail}' to BCC for Rental ID {RentalId}.", approverEmail, rental.Id);
                                    }
                                    else
                                    {
                                        _logger.LogWarning("Approver email not found for Rental ID {RentalId} (ApprovedBy: '{ApprovedBy}'), skipping BCC for this approver.", rental.Id, rental.ApprovedBy);
                                    }
                                }
                                else if (string.IsNullOrEmpty(rental.ApprovedBy))
                                {
                                    _logger.LogDebug("Rental ID {RentalId} has no 'ApprovedBy' value, skipping approver email for BCC.", rental.Id);
                                }

                                if (rental.User != null && !string.IsNullOrEmpty(rental.User.Email) && rental.Book != null && !string.IsNullOrEmpty(rental.Book.Title))
                                    {

                                        await notificationService.SendRentalReminderAsync(
                                            recipientEmail: rental.User.Email,
                                            bookTitle: rental.Book.Title,
                                            expectedReturnDate: rental.ExpectedReturnDate,
                                            bccEmails:bccEmailsForThisReminder
                                        );
                                   
                                        rental.LastReminderEmailSentDate = date;
                                        _logger.LogInformation("Sent 'due soon' reminder email for Rental ID {RentalId} ({BookTitle}) to {UserEmail}. LastReminderEmailSentDate updated.", rental.Id, rental.Book.Title, rental.User.Email);

                                    }
                                    else
                                    {
                                        _logger.LogWarning("Skipping notification for rental ID {RentalId} due to missing customer email or book title.", rental.Id);
                                    }
                                }
                                
                            
                            await context.SaveChangesAsync(stoppingToken);

                        }
                        else
                        {
                            _logger.LogInformation("No rentals found due in ~2 days.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred executing Rental Reminder Background Service.");
                }

                await Task.Delay(TimeSpan.FromMinutes(_settings.IntervalMinutes), stoppingToken);
            }

            _logger.LogInformation("Rental Reminder Background Service is stopping.");
        }
    }
}
