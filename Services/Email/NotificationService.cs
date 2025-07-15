
namespace BookStoreApi.Services.Email
{
    public class NotificationService : INotificationService
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(IEmailService emailService, ILogger<NotificationService> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }

        public async Task SendRentalReminderAsync(string recipientEmail, string bookTitle, DateTime? expectedReturnDate, ICollection<string> emails)
        {
            if (string.IsNullOrEmpty(recipientEmail))
            {
                _logger.LogWarning("Skipping rental reminder: Recipient email is null or empty for book '{BookTitle}'.", bookTitle);
                return;
            }

            try
            {
                string subject = $"Reminder: Your Book Rental for '{bookTitle}' is Due Soon!";
                string message = $@"
                    <html>
                    <head>
                        <style>
                            body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                            .container {{ max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd; border-radius: 8px; background-color: #f9f9f9; }}
                            .header {{ background-color: #4CAF50; color: white; padding: 10px 20px; text-align: center; border-radius: 8px 8px 0 0; }}
                            .content {{ padding: 20px; }}
                            .footer {{ text-align: center; font-size: 0.8em; color: #777; margin-top: 20px; border-top: 1px solid #eee; padding-top: 10px; }}
                            .highlight {{ color: #007bff; font-weight: bold; }}
                        </style>
                    </head>
                    <body>
                        <div class=""container"">
                            <div class=""header"">
                                <h2>Book Rental Reminder</h2>
                            </div>
                            <div class=""content"">
                                <p>Dear Valued Customer,</p>
                                <p>This is a friendly reminder that your rental for the book <span class=""highlight"">'{bookTitle}'</span> is due to be returned on <span class=""highlight"">{expectedReturnDate:dddd, MMMM dd, yyyy}</span> (in 2 days).</p>
                                <p>Please ensure the book is returned by the due date to avoid any late fees.</p>
                                <p>If you have already returned the book, please disregard this message.</p>
                                <p>Thank you for choosing our BookStore!</p>
                                <p>Sincerely,<br>The BookStore Team</p>
                            </div>
                            <div class=""footer"">
                                <p>&copy; {DateTime.Now.Year} BookStore. All rights reserved.</p>
                            </div>
                        </div>
                    </body>
                    </html>";

                await _emailService.SendEmailAsync(recipientEmail, subject, message,emails);
                _logger.LogInformation("Rental reminder successfully delegated to IEmailService for {RecipientEmail}.", recipientEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send rental reminder for {RecipientEmail} via IEmailService.", recipientEmail);
            }
        }

    
    }
}
