using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ethereal.Application.Extensions;
using Ethereal.Domain.Entities;
using YoutubeExplode;

namespace Ethereal.Application.Queries
{
    public class GetYoutubeVideoInfoQuery
    {
        private readonly YoutubeClient youtubeClient;

        public GetYoutubeVideoInfoQuery(YoutubeClient youtubeClient)
        {
            this.youtubeClient = youtubeClient;
        }
        
        public async Task<ProcessingJobVideo> ExecuteAsync(string url)
        {
            var youtubeVideo = await youtubeClient.Videos.GetAsync(url);
            return new ProcessingJobVideo
            {
                Description = youtubeVideo.Description,
                Title = youtubeVideo.Title,
            };
        }
    }
}