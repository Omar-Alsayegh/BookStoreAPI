namespace BookStoreApi.Services
{
    public interface IRentalService
    {
        DateTime FixTime(DateTime time, int? days);
    }
}
