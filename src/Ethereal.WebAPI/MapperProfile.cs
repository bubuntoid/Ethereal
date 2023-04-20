using AutoMapper;
using Ethereal.Application;
using Ethereal.Domain.Entities;
using Ethereal.WebAPI.Contracts;

namespace Ethereal.WebAPI;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<ProcessingJob, ProcessingJobDto>()
            .ForMember(s => s.Chapters, cfg => cfg.Ignore())
            .ForMember(s => s.LogFileUrl, cfg => cfg.Ignore())
            .ForMember(s => s.ZipArchiveUrl, cfg => cfg.Ignore());

        CreateMap<ProcessingJobVideo, ProcessingJobVideoDto>();

        CreateMap<VideoChapter, VideoChapterDto>()
            .ForMember(s => s.Mp3Url, cfg => cfg.Ignore())
            .ForMember(s => s.ThumbnailUrl, cfg => cfg.Ignore());
    }
}