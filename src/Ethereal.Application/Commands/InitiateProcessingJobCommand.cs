using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Ethereal.Application.BackgroundJobs;
using Ethereal.Domain;
using Ethereal.Domain.Entities;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using YoutubeExplode;

namespace Ethereal.Application.Commands
{
    public class InitiateProcessingJobCommand
    {
        private readonly EtherealDbContext dbContext;
        private readonly IEtherealSettings settings;
        private readonly FetchYoutubeVideoJob fetchYoutubeVideoJob;
        private readonly IBackgroundJobClient backgroundJobClient;
        private readonly YoutubeClient youtubeClient;

        public InitiateProcessingJobCommand(EtherealDbContext dbContext, IEtherealSettings settings,
            FetchYoutubeVideoJob fetchYoutubeVideoJob, IBackgroundJobClient backgroundJobClient,
            YoutubeClient youtubeClient)
        {
            this.dbContext = dbContext;
            this.settings = settings;
            this.fetchYoutubeVideoJob = fetchYoutubeVideoJob;
            this.backgroundJobClient = backgroundJobClient;
            this.youtubeClient = youtubeClient;
        }

        public async Task<Guid> ExecuteAsync(string url)
        {
            var youtubeVideo = await youtubeClient.Videos.GetAsync(url);
            if (youtubeVideo == null)
                throw new Exception("Could not get youtube video");

            var existingJob = await dbContext.ProcessingJobs
                .Where(j => j.Status != ProcessingJobStatus.Expired)
                .Where(j => j.Status != ProcessingJobStatus.Failed)
                .FirstOrDefaultAsync(j => j.VideoUrl == youtubeVideo.Url);
            
            if (existingJob != null)
                return existingJob.Id;
            
            var job = new ProcessingJob
            {
                Id = Guid.NewGuid(),
                VideoId = youtubeVideo.Id,
                VideoUrl = url,
                VideoTitle = youtubeVideo.Title,
                VideoDescription = youtubeVideo.Description,
                Status = ProcessingJobStatus.Created,
            };
            
            job.LocalPath = Path.Combine(settings.TempPath, $"{job.Id}");
            Directory.CreateDirectory(job.LocalPath);
            
            await dbContext.ProcessingJobs.AddAsync(job);
            await dbContext.SaveChangesAsync();

            backgroundJobClient.Enqueue<FetchYoutubeVideoJob>(bgJob => bgJob.Execute(job.Id));

            return job.Id;
        }
    }
}