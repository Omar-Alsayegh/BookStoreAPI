using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BookStoreApi.Infrastructure
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception,"An Exception has occured: {Message}", exception.Message);
            var ProblemDetails = new ProblemDetails
            {
                Status = httpContext.Response.StatusCode,
                Title = "An Error has Occured! ",
                Detail = exception.Message
            };
            if (exception is ArgumentException || exception is FormatException)
            {
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
             
            }
            else if (exception is UnauthorizedAccessException)
            {
                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            }
            else if (exception is NotFound)
            {
                httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
            }
            else
            {
                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            }

            await httpContext.Response.WriteAsJsonAsync(ProblemDetails, cancellationToken);
            return true;
            
        }
    }
}
