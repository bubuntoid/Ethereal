using System;

namespace Ethereal.WebAPI.Contracts.Infrastructure;

public class GuidResponseDto
{
    public GuidResponseDto()
    {
    }

    public GuidResponseDto(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; set; }
}