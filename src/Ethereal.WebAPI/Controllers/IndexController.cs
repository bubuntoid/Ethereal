using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

namespace Ethereal.WebAPI.Controllers;

public class IndexController : ControllerBase
{
    public IndexController()
    {
        
    }

    [Route(""), HttpGet]
    public Task<IActionResult> Index()
    {
        return StaticFile("index.html");
    }
    
    [HttpGet("{fileName}")]
    public async Task<IActionResult> StaticFile(string fileName)
    {
        var content = await System.IO.File.ReadAllTextAsync($@"./Frontend/{fileName}");
        new FileExtensionContentTypeProvider().TryGetContentType(fileName, out var contentType);
        return Content(content, contentType!);
    }
}