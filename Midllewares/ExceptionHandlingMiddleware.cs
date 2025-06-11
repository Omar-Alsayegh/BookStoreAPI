//using BookStoreApi.Models.DTOs; // For ErrorDetails DTO
//using Microsoft.AspNetCore.Http; // For HttpContext
//using Microsoft.Extensions.Logging; // For ILogger
//using System.Net; // For HttpStatusCode
//using System.Threading.Tasks;

//namespace BookStoreApi.Middlewares
//{
//    public class ExceptionHandlingMiddleware
//    {
//        private readonly RequestDelegate _next;
//        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

//        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
//        {
//            _next = next;
//            _logger = logger;
//        }

//        public async Task InvokeAsync(HttpContext httpContext)
//        {
//            try
//            {
//                await _next(httpContext); // Continue to the next middleware in the pipeline
//            }
//            catch (Exception ex)
//            {
//                // Catch any unhandled exception
//                _logger.LogError(ex, "An unhandled exception occurred during request processing.");
//                await HandleExceptionAsync(httpContext, ex);
//            }
//        }

//        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
//        {
//            context.Response.ContentType = "application/json";
//            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError; // Default to 500 Internal Server Error

//            // You could customize the message based on exception type if needed
//            var errorDetails = new ErrorDetails
//            {
//                StatusCode = context.Response.StatusCode,
//                // In production, you might return a more generic message
//                // e.g., "An unexpected error occurred."
//                Message = "An internal server error occurred."
//            };

//            // For development, you might include the exception message itself:
//            // if (context.RequestServices.GetService<Microsoft.AspNetCore.Hosting.IWebHostEnvironment>()?.IsDevelopment() == true)
//            // {
//            //     errorDetails.Message = exception.Message;
//            // }

//            return context.Response.WriteAsync(errorDetails.ToString());
//        }
//    }

//    // Extension method to easily add the middleware in Program.cs
//    //public static class ExceptionHandlingMiddlewareExtensions
//    {
//        public static IApplicationBuilder UseCustomExceptionHandling(this IApplicationBuilder builder)
//        {
//            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
//        }
//    }
//}