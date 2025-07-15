namespace BookStoreApi.Configurations
{
    public class RentalReminderServiceSettings
    {
        public int InitialDelayMinutes { get; set; } = 0; 
        public int IntervalMinutes { get; set; } = 5;
    }
}
