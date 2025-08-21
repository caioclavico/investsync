using Microsoft.AspNetCore.Mvc;

namespace InvestSync.Api.src.Controllers
{
    [ApiController]
    [Route("")]
    public class HealthController : ControllerBase
    {
        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new
            {
                status = "healthy",
                timestamp = DateTime.UtcNow,
                service = "InvestSync API"
            });
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return Ok(new
            {
                message = "InvestSync API is running",
                timestamp = DateTime.UtcNow,
                endpoints = new[] {
                    "/health",
                    "/swagger",
                    "/transactions",
                    "/auth"
                }
            });
        }
    }
}
