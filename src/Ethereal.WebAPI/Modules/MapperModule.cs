using System;
using AutoMapper;
using Ethereal.Application;
using Ethereal.WebAPI.Controllers.MainController.Models;

namespace Ethereal.WebAPI.Modules
{
    public class MapperModule : Profile
    {
        public MapperModule()
        {
            CreateMap<VideoChapter, VideoChapterDto>()
                .ForMember(c => c.StartTimespan, cfg => cfg.MapFrom(c => c.StartTimespan.ToString()))
                .ForMember(c => c.Duration, cfg => cfg.MapFrom(c => c.Duration.ToString()))
                .ForMember(c => c.EndTimespan, cfg => cfg.MapFrom(c => c.EndTimespan.ToString()))
                .ForMember(c => c.ThumbnailBase64, cfg =>
                {
                    cfg.PreCondition(c => c.Thumbnail != null);
                    cfg.MapFrom(c => Convert.ToBase64String(c.Thumbnail));
                });
        }
    }
}