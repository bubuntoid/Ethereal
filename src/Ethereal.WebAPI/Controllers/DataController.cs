using System;
using System.IO;
using System.Threading.Tasks;
using Autofac;
using Ethereal.Application.Queries;
using Ethereal.WebAPI.Contracts.Infrastructure;
using Ethereal.WebAPI.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Ethereal.WebAPI.Controllers;

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

    [HttpGet("{jobId}/logs")]
    [ProducesResponseType(typeof(string), 200)]
    [ProducesResponseType(typeof(ErrorResponseDto), 400)]
    public async Task<IActionResult> DownloadLogFile(Guid jobId, bool inline = true)
    {
        var path = await scope.Resolve<GetLogFilePathQuery>()
            .ExecuteAsync(jobId);

        return inline
            ? Ok(await System.IO.File.ReadAllTextAsync(path))
            : PhysicalFile(path, "text/plain", $"{jobId}.txt", true);
    }

    [HttpGet("{jobId}/zip")]
    [Produces("application/zip")]
    [ProducesResponseType(typeof(ErrorResponseDto), 400)]
    public async Task<IActionResult> DownloadZipArchive(Guid jobId)
    {
        var path = await scope.Resolve<GetZipArchiveFilePathQuery>()
            .ExecuteAsync(jobId);

        return PhysicalFile(path, "application/zip", Path.GetFileName(path), true);
    }

    [HttpGet("{jobId}/{index}/mp3")]
    [Produces("audio/mp3")]
    [ProducesResponseType(typeof(ErrorResponseDto), 400)]
    public async Task<IActionResult> DownloadChapterMp3(Guid jobId, int index, bool inline = true)
    {
        var path = await scope.Resolve<GetAudioFilePathQuery>()
            .ExecuteAsync(jobId, index);

        return inline
            ? PhysicalFile(path, "audio/mp3", true)
            : PhysicalFile(path, "audio/mp3", Path.GetFileName(path).Replace(".mp4", ".mp3"),
                true); // todo: clean extensions casting
    }

    [HttpGet("{jobId}/{index}/thumbnail")]
    [Produces("image/png")]
    [ProducesResponseType(typeof(ErrorResponseDto), 400)]
    public async Task<IActionResult> DownloadChapterThumbnail(Guid jobId, int index, bool inline = true)
    {
        var path = await scope.Resolve<GetThumbnailFilePathQuery>()
            .ExecuteAsync(jobId, index);

        return inline
            ? PhysicalFile(path, "image/png", true)
            : PhysicalFile(path, "image/png", Path.GetFileName(path), true);
    }
}