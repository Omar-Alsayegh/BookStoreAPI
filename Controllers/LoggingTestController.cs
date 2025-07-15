using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoggingTestController : ControllerBase
    {
        private readonly ILogger<LoggingTestController> _logger;

        public LoggingTestController(ILogger<LoggingTestController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            _logger.LogTrace("This is a Trace log");
            _logger.LogDebug("This is a debug log");
            _logger.LogInformation("This is an Information log.");
            _logger.LogWarning("This is a Warning log. Something might be wrong.");
            _logger.LogError(new Exception("Example exception"), "This is an Error log with an exception.");
            _logger.LogCritical("This is a Critical log. Application might be crashing!");

            return Ok("Logs sent!");
        }

    }
}
