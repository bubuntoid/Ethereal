using System;
using System.IO;
using System.Threading.Tasks;
using Autofac;
using Ethereal.Application.Queries;
using Ethereal.WebAPI.Contracts.Infrastructure;
using Ethereal.WebAPI.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileSystemGlobbing;

namespace Ethereal.WebAPI.Controllers
{
    [ApiController]
    [Route("dl")]
    [ServiceFilter(typeof(ExceptionFilter))]
    public class DataController : ControllerBase
    {
        private readonly ILifetimeScope scope;

        public DataController(ILifetimeScope scope)
        {
            this.scope = scope;
        }

        [HttpGet("{jobId}")]
        [Produces("application/zip")]
        [ProducesResponseType(typeof(ErrorResponseDto), 400)]
        public async Task<IActionResult> DownloadZipArchive(Guid jobId)
        {
            var path = await scope.Resolve<GetZipArchiveFilePathQuery>()
                .ExecuteAsync(jobId);

            return PhysicalFile(path, "application/zip", Path.GetFileName(path), true);
        }
        
        [HttpGet("{jobId}/{index}")]
        [Produces("application/zip")]
        [ProducesResponseType(typeof(ErrorResponseDto), 400)]
        public async Task<IActionResult> DownloadChapterMp3(Guid jobId, int index)
        {
            var path = await scope.Resolve<GetAudioFilePathQuery>()
                .ExecuteAsync(jobId, index);

            return PhysicalFile(path, "application/octet-stream", Path.GetFileName(path), true);
        }
        
    }
}