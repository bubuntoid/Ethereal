using Ethereal.WebAPI.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Ethereal.WebAPI.Controllers
{
    [ApiController]
    [Route("api")]
    [ServiceFilter(typeof(ExceptionFilter))]
    public class MetaController : ControllerBase
    {
        [HttpGet("hc")]
        public IActionResult HealthCheck()
        {
            return Ok(new {status = "ok"});
        }
    }
}