using System;

namespace Ethereal.WebAPI.Contracts.Infrastructure
{
    public class GuidResponseDto
    {
        public Guid Id { get; set; }

        public GuidResponseDto()
        {
            
        }

        public GuidResponseDto(Guid id)
        {
            Id = id;
        }
    }
}