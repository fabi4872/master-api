using Microsoft.AspNetCore.Mvc;

namespace MasterApi.Api.Controllers
{
    [ApiController]
    public class HealthController : ControllerBase
    {
        [HttpGet("/health")] // Explicitly define the route for this action
        public IActionResult GetHealth()
        {
            return Ok("API is healthy. Swagger and pipeline working!");
        }
    }
}
