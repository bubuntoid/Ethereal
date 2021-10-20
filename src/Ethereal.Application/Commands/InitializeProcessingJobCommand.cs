using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Ethereal.Application.BackgroundJobs;
using Ethereal.Application.Exceptions;
using Ethereal.Application.Extensions;
using Ethereal.Application.ProcessingJobLogger;
using Ethereal.Domain;
using Ethereal.Domain.Entities;
using Hangfire;
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

            if (youtubeVideo.Duration.HasValue == false || youtubeVideo.Duration.Value == TimeSpan.Zero)
                throw new InternalErrorException("Live streams not supported");

            if (youtubeVideo.Duration.Value > settings.VideoDurationLimit)
                throw new InternalErrorException($"Video duration exceeded time limit ({settings.VideoDurationLimit})");
                
            var desc = description ?? youtubeVideo.Description;
            // var existingJob = await dbContext.ProcessingJobs
            //     .Where(j => j.Status != ProcessingJobStatus.Expired)
            //     .Where(j => j.Status != ProcessingJobStatus.Failed)
            //     .Where(j => j.Video.Description == desc)
            //     .FirstOrDefaultAsync(j => j.Video.Id == youtubeVideo.Id.Value);
            // 
            // if (existingJob != null)
            //     return existingJob.Id;
            
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

            var chapters = job.ParseChapters();
            if (chapters?.Any() == false)
                throw new NotFoundException("Chapters not found");
            
            job.LocalPath = Path.Combine(settings.TempPath, $"{job.Id}");
            Directory.CreateDirectory(job.LocalPath);
            
            await dbContext.ProcessingJobs.AddAsync(job);
            await dbContext.SaveChangesAsync();

            await job.LogAsync("Job created");
            backgroundJobClient.Enqueue<InitializeJob>(bgJob => bgJob.Execute(job.Id));

            return job.Id;
        }
    }
}