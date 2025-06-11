namespace BookStoreApi.Extra
{
    public class AuthorQueryObject
    {
        public string? Name { get; set; }
        public DateTime? Birthdate { get; set; }
        public string? SortBy { get; set; }
        public bool IsDescending { get; set; } = false;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
