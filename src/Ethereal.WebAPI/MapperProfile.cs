using System;
using AutoMapper;
using Ethereal.Application;
using Ethereal.Domain.Entities;
using Ethereal.WebAPI.Contracts;

namespace Ethereal.WebAPI
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<ProcessingJob, ProcessingJobDto>();
            
            CreateMap<ProcessingJobVideo, ProcessingJobVideoDto>();
        }
    }
}