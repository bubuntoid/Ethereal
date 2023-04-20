using System.Threading.Tasks;
using Ethereal.Domain;
using Ethereal.Domain.Entities;
using Ethereal.WebAPI.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ethereal.WebAPI.Controllers;

[ApiController]
[Route("api")]
[ServiceFilter(typeof(ExceptionFilter))]
public class MetaController : ControllerBase
{
    private readonly EtherealDbContext dbContext;

    public MetaController(EtherealDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    [HttpGet("hc")]
    public async Task<IActionResult> HealthCheck()
    {
        return Ok(new
        {
            status = "ok",
            totalJobs = await dbContext.ProcessingJobs.CountAsync(),
            succeedJobs = await dbContext.ProcessingJobs.CountAsync(s => s.Status == ProcessingJobStatus.Succeed)
        });
    }
}