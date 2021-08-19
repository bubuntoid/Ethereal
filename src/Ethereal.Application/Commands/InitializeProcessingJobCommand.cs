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
    public class InitializeProcessingJobCommand
    {
        private readonly EtherealDbContext dbContext;
        private readonly IEtherealSettings settings;
        private readonly IBackgroundJobClient backgroundJobClient;
        private readonly YoutubeClient youtubeClient;

        public InitializeProcessingJobCommand(EtherealDbContext dbContext, IEtherealSettings settings,
            IBackgroundJobClient backgroundJobClient,
            YoutubeClient youtubeClient)
        {
            this.dbContext = dbContext;
            this.settings = settings;
            this.backgroundJobClient = backgroundJobClient;
            this.youtubeClient = youtubeClient;
        }

        public async Task<Guid> ExecuteAsync(string url, string description = null)
        {
            var youtubeVideo = await youtubeClient.Videos.GetAsync(url);
            if (youtubeVideo == null)
                throw new Exception("Could not get youtube video");

            var desc = description ?? youtubeVideo.Description;
            var existingJob = await dbContext.ProcessingJobs
                .Where(j => j.Status != ProcessingJobStatus.Expired)
                .Where(j => j.Status != ProcessingJobStatus.Failed)
                .Where(j => j.Video.Description == desc)
                .FirstOrDefaultAsync(j => j.Video.Url == youtubeVideo.Url);
            
            if (existingJob != null)
                return existingJob.Id;
            
            var job = new ProcessingJob
            {
                Id = Guid.NewGuid(),
                Status = ProcessingJobStatus.Created,
                
                Video = new ProcessingJobVideo
                {
                    Id = youtubeVideo.Id,
                    Url = url,
                    Title = youtubeVideo.Title,
                    OriginalDescription = desc,
                    Description = desc,
                },
            };
            
            job.LocalPath = Path.Combine(settings.TempPath, $"{job.Id}");
            Directory.CreateDirectory(job.LocalPath);
            
            await dbContext.ProcessingJobs.AddAsync(job);
            await dbContext.SaveChangesAsync();

            backgroundJobClient.Enqueue<InitializeJob>(bgJob => bgJob.Execute(job.Id));

            return job.Id;
        }
    }
}