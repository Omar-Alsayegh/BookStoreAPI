namespace BookStoreApi.Extra
{
    public interface IQueryObject
    {
        string? SortBy { get; set; }
        bool IsDescending { get; set; }
        int PageNumber { get; set; }
        int PageSize { get; set; }
    }
}