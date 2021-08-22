using System;
using System.Threading.Tasks;
using Autofac;
using Ethereal.Application.Commands;
using Ethereal.WebAPI.Contracts;
using Ethereal.WebAPI.Contracts.Infrastructure;
using Ethereal.WebAPI.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Ethereal.WebAPI.Controllers
{
    [ApiController]
    [Route("api")]
    [ServiceFilter(typeof(ExceptionFilter))]
    public class MainController : ControllerBase
    {
        private readonly ILifetimeScope scope;

        public MainController(ILifetimeScope scope)
        {
            this.scope = scope;
        }

        [HttpPost("initialize")]
        [ProducesResponseType(typeof(GuidResponseDto), 200)]
        [ProducesResponseType(typeof(ErrorResponseDto), 400)]
        public async Task<IActionResult> Initialize(InitializeJobRequestDto dto)
        {
            var id = await scope.Resolve<InitializeProcessingJobCommand>()
                .ExecuteAsync(dto.Url, dto.Description);
            
            return Ok(new GuidResponseDto(id));
        }
    }
}