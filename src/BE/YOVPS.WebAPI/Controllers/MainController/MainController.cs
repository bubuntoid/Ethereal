using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NLog;
using YOVPS.Core;
using YOVPS.Core.Extensions;
using YOVPS.WebAPI.Controllers.MainController.Models;
using YOVPS.WebAPI.Filters;
using YOVPS.WebAPI.Models;

namespace YOVPS.WebAPI.Controllers.MainController
{
    [ApiController]
    [Route("api")]
    [ServiceFilter(typeof(ExceptionFilter))]
    public class MainController : ControllerBase
    {
        private readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly IVideoSplitterService splitter;

        public MainController(IVideoSplitterService splitter)
        {
            this.splitter = splitter;
        }

        [HttpGet("download/zip")]
        [Produces("application/zip")]
        [ProducesResponseType(typeof(ApiErrorDto), 400)]
        public async Task<IActionResult> DownloadZip(DownloadZipRequestDto dto)
        {
            var watch = new Stopwatch();
            watch.Start();
            var result = await splitter.DownloadZipAsync(dto.Url, dto.Description);
            watch.Stop();
            logger.Info($"download/zip took {watch.Elapsed.TotalSeconds} seconds");
            
            return File(result.Object, "application/zip", result.Name);
        }

        [HttpGet("download/mp3")]
        [Produces("application/octet-stream")]
        [ProducesResponseType(typeof(ApiErrorDto), 400)]
        public async Task<IActionResult> DownloadMp3(DownloadMp3RequestDto dto)
        {
            var watch = new Stopwatch();
            watch.Start();
            var result = await splitter.DownloadMp3Async(dto.Url, dto.Description, dto.Index.Value);
            watch.Stop();
            logger.Info($"download/mp3 took {watch.Elapsed.TotalSeconds} seconds");
            
            return File(result.Object, "application/octet-stream", result.Name);
        }

        [HttpGet("download/thumbnail")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(ApiErrorDto), 400)]
        public async Task<IActionResult> DownloadThumbnail(string url)
        {
            return Ok(await splitter.GetThumbnailUrlAsync(url));
        }
        
        [HttpGet("chapters")]
        [ProducesResponseType(typeof(IEnumerable<VideoChapterDto>), 200)]
        [ProducesResponseType(typeof(ApiErrorDto), 400)]
        public async Task<IActionResult> GetChaptersByUrl(GetChaptersByUrlRequestDto dto)
        {
            // todo: add mapper
            var chapters = await splitter.GetChaptersAsync(dto.Url, dto.Description, dto.IncludeThumbnails);
            var mapped = chapters.Select(chapter => new VideoChapterDto
            {
                Name = chapter.Name,
                StartTimespan = chapter.StartTimespan.ToString(),
                EndTimespan = chapter.EndTimespan.ToString(),
                Duration = chapter.Duration.ToString(),
                Original = chapter.Original,
                ThumbnailBase64 = chapter.Thumbnail != null 
                    ? Convert.ToBase64String(chapter.Thumbnail)
                    : null
            });
            
            return Ok(mapped);
        }
    }
}