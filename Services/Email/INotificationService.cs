namespace BookStoreApi.Services.Email
{
    public interface INotificationService
    {
        Task SendRentalReminderAsync(string recipientEmail, string bookTitle, DateTime? expectedReturnDate,ICollection<string>bccEmails);
    }
}
