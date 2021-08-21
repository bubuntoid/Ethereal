using Ethereal.WebAPI.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Ethereal.WebAPI.Controllers
{
    [ApiController]
    [Route("dl")]
    [ServiceFilter(typeof(ExceptionFilter))]
    public class DataController
    {
        
    }
}