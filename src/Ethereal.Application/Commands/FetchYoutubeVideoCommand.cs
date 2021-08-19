using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Ethereal.Application.Extensions;
using Ethereal.Domain;
using Ethereal.Domain.Entities;
using YoutubeExplode;

namespace Ethereal.Application.Commands
{
    public class FetchYoutubeVideoCommand
    {
        private readonly EtherealDbContext dbContext;
        private readonly YoutubeClient youtubeClient;

        public FetchYoutubeVideoCommand(EtherealDbContext dbContext, YoutubeClient youtubeClient)
        {
            this.dbContext = dbContext;
            this.youtubeClient = youtubeClient;
        }
        
        public async Task ExecuteAsync(ProcessingJob job)
        {
            job.Status = ProcessingJobStatus.FetchingVideo;
            job.CurrentStepDescription = "Fetching video from youtube";
            job.TotalStepsCount = 1;
            job.CurrentStepIndex = 0;
            await dbContext.SaveChangesAsync();
            
            // Fetching video from youtube
            var manifest = await youtubeClient.Videos.Streams.GetManifestAsync(job.Video.Id);
            var manifestStreams = manifest.GetVideoStreams();
            var videoStreamInfo = manifestStreams.FirstOrDefault(x => x.Container.Name == "mp4");
            await using var videoStream = await youtubeClient.Videos.Streams.GetAsync(videoStreamInfo);
            
            // Saving video locally
            await using var createdFileStream = File.Create(job.GetLocalVideoPath());
            videoStream.Seek(0, SeekOrigin.Begin);
            await videoStream.CopyToAsync(createdFileStream);
            
            job.CurrentStepIndex++;
            job.CurrentStepDescription = "Video fetched";
            await dbContext.SaveChangesAsync();
        }
    }
}