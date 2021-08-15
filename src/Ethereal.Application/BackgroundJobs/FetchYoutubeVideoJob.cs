using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Ethereal.Application.Commands;
using Ethereal.Domain;
using Ethereal.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using YoutubeExplode;

namespace Ethereal.Application.BackgroundJobs
{
    public class FetchYoutubeVideoJob : BackgroundJobBase<Guid>
    {
        private readonly EtherealDbContext dbContext;
        private readonly YoutubeClient youtubeClient;

        public FetchYoutubeVideoJob(EtherealDbContext dbContext, YoutubeClient youtubeClient)
        {
            this.dbContext = dbContext;
            this.youtubeClient = youtubeClient;
        }

        public override async Task ExecuteAsync(Guid jobId)
        {
            var job = await dbContext.ProcessingJobs.FirstOrDefaultAsync(j => j.Id == jobId);
            if (job == null)
            {
                // todo: log
                return;
            }

            var chapters = new VideoDescription(job.VideoDescription).ParseChapters();
            if (chapters?.Any() == false)
                throw new Exception();
            
            await FetchAsync(job);
            await FetchThumbnailsAsync(job, chapters);
        }

        private async Task FetchAsync(ProcessingJob job)
        {
            job.Status = ProcessingJobStatus.FetchingVideo;
            job.CurrentProcessingStep = "Fetching video from youtube";
            await dbContext.SaveChangesAsync();
            
            // Fetching video from youtube
            var manifest = await youtubeClient.Videos.Streams.GetManifestAsync(job.VideoId);
            var manifestStreams = manifest.GetVideoStreams();
            var videoStreamInfo = manifestStreams.FirstOrDefault(x => x.Container.Name == "mp4");
            await using var videoStream = await youtubeClient.Videos.Streams.GetAsync(videoStreamInfo);
            
            // Saving video locally
            var path = Path.Combine(job.LocalPath, EtherealConstants.OriginalVideoFileName);
            await using var createdFileStream = File.Create(path);
            videoStream.Seek(0, SeekOrigin.Begin);
            await videoStream.CopyToAsync(createdFileStream);
            
            job.CurrentProcessingStep = "Video fetched";
            await dbContext.SaveChangesAsync();
        }

        private async Task FetchThumbnailsAsync(ProcessingJob job, IEnumerable<VideoChapter> chapters)
        {
            foreach (var chapter in chapters)
            {
                
            }
        }
    }
}