using System;
using System.Threading.Tasks;
using Autofac;
using AutoMapper;
using Ethereal.Application.Commands;
using Ethereal.Application.Queries;
using Ethereal.WebAPI.Contracts;
using Ethereal.WebAPI.Contracts.Infrastructure;
using Ethereal.WebAPI.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Ethereal.WebAPI.Controllers
{
    [ApiController]
    [Route("api/jobs")]
    [ServiceFilter(typeof(ExceptionFilter))]
    public class MainController : ControllerBase
    {
        private readonly ILifetimeScope scope;
        private readonly IMapper mapper;

        public MainController(ILifetimeScope scope, IMapper mapper)
        {
            this.scope = scope;
            this.mapper = mapper;
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
        
        [HttpGet("{jobId}")]
        [ProducesResponseType(typeof(ProcessingJobDto), 200)]
        [ProducesResponseType(typeof(ErrorResponseDto), 400)]
        public async Task<IActionResult> Get(Guid jobId)
        {
            var job = await scope.Resolve<GetProcessingJobQuery>()
                .ExecuteAsync(jobId);

            return Ok(mapper.Map<ProcessingJobDto>(job));
        }
    }
}