﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using AutoMapper;
using Ethereal.Application.Queries;
using Ethereal.WebAPI.Contracts;
using Ethereal.WebAPI.Contracts.Infrastructure;
using Ethereal.WebAPI.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Ethereal.WebAPI.Controllers;

[ApiController]
[Route("api/preview")]
[ServiceFilter(typeof(ExceptionFilter))]
public class PreviewController : ControllerBase
{
    private readonly IMapper mapper;
    private readonly ILifetimeScope scope;

    public PreviewController(ILifetimeScope scope, IMapper mapper)
    {
        this.scope = scope;
        this.mapper = mapper;
    }

    [HttpPost("description")]
    [ProducesResponseType(typeof(string), 200)]
    [ProducesResponseType(typeof(ErrorResponseDto), 400)]
    public async Task<IActionResult> GetYoutubeDescription(GetYoutubeDescriptionRequestDto dto)
    {
        var video = await scope.Resolve<GetYoutubeVideoInfoQuery>().ExecuteAsync(dto.Url);
        return Ok(video.Description);
    }

    [HttpPost("chapters")]
    [ProducesResponseType(typeof(IReadOnlyCollection<VideoChapterDto>), 200)]
    [ProducesResponseType(typeof(ErrorResponseDto), 400)]
    public async Task<IActionResult> GetChaptersPreview(InitializeJobRequestDto dto)
    {
        var chapters = await scope.Resolve<GetVideoChaptersPreviewQuery>().ExecuteAsync(dto.Url, dto.Description);
        return Ok(chapters.Select(s => mapper.Map<VideoChapterDto>(s)));
    }
}