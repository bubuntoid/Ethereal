using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Ethereal.Application;
using Ethereal.WebAPI.Controllers.MainController.Models;
using Ethereal.WebAPI.Filters;
using Ethereal.WebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace Ethereal.WebAPI.Controllers.MainController
{
    [ApiController]
    [Route("api")]
    [ServiceFilter(typeof(ExceptionFilter))]
    public class MainController : ControllerBase
    {
        // private readonly Logger logger = LogManager.GetCurrentClassLogger();
        // private readonly IVideoSplitterService splitter;
        // private readonly IMapper mapper;
        // 
        // public MainController(IVideoSplitterService splitter, IMapper mapper)
        // {
        //     this.splitter = splitter;
        //     this.mapper = mapper;
        // }
        // 
        // [HttpPost("download/zip")]
        // [Produces("application/zip")]
        // [ProducesResponseType(typeof(ApiErrorDto), 400)]
        // public async Task<IActionResult> DownloadZip(DownloadZipRequestDto dto)
        // {
        //     var result = await splitter.DownloadZipAsync(dto.Url, dto.Description);
        //     return File(result.Object, "application/zip", result.Name);
        // }
        // 
        // [HttpPost("download/mp3")]
        // [Produces("application/octet-stream")]
        // [ProducesResponseType(typeof(ApiErrorDto), 400)]
        // public async Task<IActionResult> DownloadMp3(DownloadMp3RequestDto dto)
        // {
        //     var result = await splitter.DownloadMp3Async(dto.Url, dto.Description, dto.Index.Value);
        //     return File(result.Object, "application/octet-stream", result.Name);
        // }
        // 
        // [HttpPost("download/thumbnail")]
        // [ProducesResponseType(typeof(string), 200)]
        // [ProducesResponseType(typeof(ApiErrorDto), 400)]
        // public async Task<IActionResult> DownloadThumbnail(DownloadThumbnailRequestDto dto)
        // {
        //     var url = await splitter.GetThumbnailUrlAsync(dto.Url); 
        //     return Ok(url);
        // }
        // 
        // [HttpPost("chapters")]
        // [ProducesResponseType(typeof(IEnumerable<VideoChapterDto>), 200)]
        // [ProducesResponseType(typeof(ApiErrorDto), 400)]
        // public async Task<IActionResult> GetChaptersByUrl(GetChaptersByUrlRequestDto dto)
        // {
        //     var chapters = await splitter.GetChaptersAsync(dto.Url, dto.Description, dto.IncludeThumbnails);
        //     return Ok(chapters.Select(c => mapper.Map<VideoChapterDto>(c)));
        // }
    }
}