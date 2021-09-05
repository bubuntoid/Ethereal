using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ethereal.Application.Extensions;
using Ethereal.Domain.Entities;
using YoutubeExplode;

namespace Ethereal.Application.Queries
{
    public class GetYoutubeDescriptionQuery
    {
        private readonly YoutubeClient youtubeClient;

        public GetYoutubeDescriptionQuery(YoutubeClient youtubeClient)
        {
            this.youtubeClient = youtubeClient;
        }
        
        public async Task<string> ExecuteAsync(string url)
        {
            var youtubeVideo = await youtubeClient.Videos.GetAsync(url);
            return youtubeVideo.Description;
        }
    }
}