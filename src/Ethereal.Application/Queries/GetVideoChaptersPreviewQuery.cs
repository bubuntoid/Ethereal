using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ethereal.Application.Extensions;
using Ethereal.Domain.Entities;
using YoutubeExplode;

namespace Ethereal.Application.Queries
{
    public class GetVideoChaptersPreviewQuery
    {
        private readonly YoutubeClient youtubeClient;

        public GetVideoChaptersPreviewQuery(YoutubeClient youtubeClient)
        {
            this.youtubeClient = youtubeClient;
        }
        
        public async Task<IReadOnlyCollection<VideoChapter>> ExecuteAsync(string url, string description = null)
        {
            var youtubeVideo = await youtubeClient.Videos.GetAsync(url);
            var desc = description ?? youtubeVideo.Description;
            
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