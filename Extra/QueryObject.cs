namespace BookStoreApi.Extra
{
    public class QueryObject
    {
        public string? Title { get; set; }
        public string? AuthorName { get; set; }
        public string? PublisherName { get; set; }

        public string? SortBy { get; set; } 
        public bool IsDescending { get; set; } = false;

        public int PageNumber { get; set; } = 1; 
        public int PageSize { get; set; } = 10;
    }
}
