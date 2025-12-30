using Microsoft.AspNetCore.Mvc;

namespace MasterApi.Api.Controllers;

[ApiController]
[Route("health")]
public class HealthController : ApiControllerBase
{
    [HttpGet]
    public IActionResult GetHealth()
    {
        return Ok("API is healthy. Swagger and pipeline working!");
    }
}
