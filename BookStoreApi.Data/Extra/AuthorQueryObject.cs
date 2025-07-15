﻿namespace BookStoreApi.Extra
{
    public class AuthorQueryObject:IQueryObject
    {
        public string? Name { get; set; }
        public DateTime? Birthdate { get; set; }

        public string? SortBy { get; set; }
        public bool IsDescending { get; set; } = false;

        private const int MaxPageSize = 50;
        public int PageNumber { get; set; } = 1;
        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
        //public string? SortBy { get; set; }
        //public bool IsDescending { get; set; } = false;
        //public int PageNumber { get; set; } = 1;
        //public int PageSize { get; set; } = 10;
    }
}
