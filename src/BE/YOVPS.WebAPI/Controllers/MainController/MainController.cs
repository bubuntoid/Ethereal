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

        // [HttpGet("stream/mp3")]
        // public async Task<IActionResult> GetAudioStream([FromQuery] VideoCredentialsDto dto)
        // {
        //     return File(await splitter.GetAudioStreamAsync(dto.Url, dto.Description, dto.Index),
        //         "application/octet-stream", true);
        // }
        
        [HttpGet("download/zip")]
        public async Task<IActionResult> DownloadZip([FromQuery] VideoCredentialsDto dto)
        {
            var result = await splitter.DownloadZipAsync(dto.Url, dto.Description);
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
        
        [HttpGet("chapters/url")]
        public async Task<IActionResult> GetChaptersByUrl(string url)
        {
            // todo: add mapper
            var chapters = await splitter.GetChaptersByUrlAsync(url);
            var mapped = chapters.Select(chapter => new VideoChapterDto
            {
                Name = chapter.ParsedName,
                Timespan = chapter.ParsedTimespan.ToString(),
            });
            
            return Ok(mapped);
        }

        [HttpGet("chapters/description")]
        public IActionResult GetChaptersByDescription(string description)
        {
            // todo: add mapper
            var chapters = splitter.GetChaptersByDescription(description);
            var mapped = chapters.Select(chapter => new VideoChapterDto
            {
                Name = chapter.ParsedName,
                Timespan = chapter.ParsedTimespan.ToString(),
            });

            return Ok(mapped);
        }
    }
}