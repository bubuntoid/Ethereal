using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using YOVPS.Core;
using YOVPS.WebAPI.Controllers.MainController.Models;

namespace YOVPS.WebAPI.Controllers.MainController
{
    [ApiController]
    [Route("api")]
    public class MainController : ControllerBase
    {
        private readonly IVideoSplitterService splitter;

        public MainController(IVideoSplitterService splitter)
        {
            this.splitter = splitter;
        }

        [HttpGet("download/zip")]
        public async Task<IActionResult> DownloadZip([FromQuery] VideoCredentialsDto dto)
        {
            var watch = new Stopwatch();
            watch.Start();
            var result = await splitter.DownloadZipAsync(dto.Url, dto.Description);
            watch.Stop();
            Console.WriteLine($"download/zip took {watch.Elapsed.TotalSeconds} seconds");
            
            return File(result.Object, "application/zip", result.Name);
        }

        [HttpGet("download/mp3")]
        public async Task<IActionResult> DownloadMp3([FromQuery] VideoCredentialsDto dto)
        {
            var result = await splitter.DownloadMp3Async(dto.Url, dto.Description, dto.Index.Value);
            return File(result.Object, "application/octet-stream", result.Name);
        }

        [HttpGet("download/thumbnail")]
        public async Task<IActionResult> DownloadThumbnail(string url)
        {
            return Ok(await splitter.GetThumbnailUrlAsync(url));
        }
        
        [HttpGet("chapters")]
        public async Task<IActionResult> GetChaptersByUrl([FromQuery] VideoCredentialsDto dto)
        {
            // todo: add mapper
            var chapters = await splitter.GetChaptersAsync(dto.Url, dto.Description);
            var mapped = chapters.Select(chapter => new VideoChapterDto
            {
                Name = chapter.Name,
                StartTimespan = chapter.StartTimespan.ToString(),
                EndTimespan = chapter.EndTimespan.ToString(),
                Duration = chapter.Duration.ToString(),
                Original = chapter.Original
            });
            
            return Ok(mapped);
        }
    }
}