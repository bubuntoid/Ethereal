using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ethereal.Application.Exceptions;
using Ethereal.Application.Extensions;
using Ethereal.Domain.Entities;
using YoutubeExplode;

namespace Ethereal.Application.Queries
{
    public class GetVideoChaptersPreviewQuery
    {
        private readonly YoutubeClient youtubeClient;
        private readonly IEtherealSettings settings;

        public GetVideoChaptersPreviewQuery(YoutubeClient youtubeClient, IEtherealSettings settings)
        {
            this.youtubeClient = youtubeClient;
            this.settings = settings;
        }
        
        public async Task<IReadOnlyCollection<VideoChapter>> ExecuteAsync(string url, string description = null)
        {
            var youtubeVideo = await youtubeClient.Videos.GetAsync(url);
            var desc = description ?? youtubeVideo.Description;
            
            if (youtubeVideo.Duration.HasValue == false || youtubeVideo.Duration.Value == TimeSpan.Zero)
                throw new InternalErrorException("Live streams not supported");

            if (youtubeVideo.Duration.Value > settings.VideoDurationLimit)
                throw new InternalErrorException($"Video duration exceeded time limit ({settings.VideoDurationLimit})");
            
            var job = new ProcessingJob
            {
                Id = Guid.NewGuid(),
                Status = ProcessingJobStatus.Created,
                CreatedDate = DateTime.UtcNow,
                
                Video = new ProcessingJobVideo
                {
                    Id = youtubeVideo.Id,
                    Url = url,
                    Title = youtubeVideo.Title,
                    OriginalDescription = youtubeVideo.Description,
                    Description = desc,
                    Duration = youtubeVideo.Duration.GetValueOrDefault(),
                },
            };

            return job.ParseChapters();
        }
    }
}