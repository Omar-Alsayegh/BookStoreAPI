using BookStoreApi.Entities;
using BookStoreApi.Models.DTOs;
using BookStoreApi.Services;
using System.ComponentModel;

namespace BookStoreApi.Sheduled_Tasks
{
    public class NewArrivalsProcessor : INewArrivalsProcessor
    {
        private readonly ILogger<NewArrivalsProcessor> _logger;
        private readonly IBookService _bookService;

        public NewArrivalsProcessor(ILogger<NewArrivalsProcessor> logger,IBookService bookService)
        {
            _logger = logger;
            _bookService = bookService;
        }

        public async Task ProcessNewArrivalsAsync()
        {
            _logger.LogInformation("Processing new book arrivals at {Timestamp}...", DateTimeOffset.Now);

            DateTimeOffset sinceDate = DateTimeOffset.UtcNow.AddDays(-2);

            IEnumerable<Book> newBooks = await _bookService.GetBooksAddedSinceAsync(sinceDate);

            if (newBooks != null && newBooks.Any())
            {
                _logger.LogInformation("Found {Count} new books added since {SinceDate}.", newBooks.Count(), sinceDate);

                foreach (var book in newBooks)
                {
                    _logger.LogInformation("   - New Book: {BookId} - {BookTitle} (Published: {PubYear})",
                        book.Id, book.Title, book.PublicationYear);
                }



                await Task.Delay(2); // Simulate some work
            _logger.LogInformation("Finished processing new book arrivals.");
        }
            else
            {
                _logger.LogInformation("No new books found since {SinceDate}.", sinceDate);
            }

            _logger.LogInformation("Finished processing new book arrivals.");
        

        }
    }
}
