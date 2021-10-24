using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Ethereal.Application.Commands;
using Ethereal.Application.Extensions;
using Ethereal.Application.ProcessingJobLogger;
using Ethereal.Domain;
using Ethereal.Domain.Entities;
using Hangfire;
using Microsoft.EntityFrameworkCore;

namespace Ethereal.Application.BackgroundJobs
{
    [AutomaticRetry(Attempts = 0)]
    public class InitializeJob : BackgroundJobBase<Guid>
    {
        private readonly EtherealDbContext dbContext;
        private readonly FetchThumbnailsCommand fetchThumbnailsCommand;
        private readonly FetchYoutubeVideoCommand fetchYoutubeVideoCommand;
        private readonly ConvertVideoCommand splitVideoCommand;
        private readonly IBackgroundJobClient backgroundJobClient;
        private readonly ArchiveFilesCommand archiveFilesCommand;

        public InitializeJob(EtherealDbContext dbContext,
            FetchThumbnailsCommand fetchThumbnailsCommand, FetchYoutubeVideoCommand fetchYoutubeVideoCommand,
            ConvertVideoCommand splitVideoCommand, IBackgroundJobClient backgroundJobClient,
            ArchiveFilesCommand archiveFilesCommand)
        {
            this.dbContext = dbContext;
            this.fetchThumbnailsCommand = fetchThumbnailsCommand;
            this.fetchYoutubeVideoCommand = fetchYoutubeVideoCommand;
            this.splitVideoCommand = splitVideoCommand;
            this.backgroundJobClient = backgroundJobClient;
            this.archiveFilesCommand = archiveFilesCommand;
        }

        public override async Task ExecuteAsync(Guid jobId)
        {
            var job = await dbContext.ProcessingJobs
                .Include(j => j.Video)
                .FirstOrDefaultAsync(j => j.Id == jobId);
            
            if (job == null)
                return;

            await job.LogAsync("Initializing chapters...");
            
            var chapters = new VideoDescription(job.Video.Description).ParseChapters();
            if (chapters?.Any() == false)
            {
                job.Status = ProcessingJobStatus.Failed;
                await dbContext.SaveChangesAsync();
                await job.LogAsync($"Could not parse any chapter");
                return;
            }

            await job.LogAsync($"Found {chapters.Count} chapters");
            
            try
            {
                await fetchYoutubeVideoCommand.ExecuteAsync(job.Id);
                await fetchThumbnailsCommand.ExecuteAsync(job.Id);
                await splitVideoCommand.ExecuteAsync(job.Id);
                await archiveFilesCommand.ExecuteAsync(job.Id);

                job.Status = ProcessingJobStatus.Succeed;
                job.ProcessedDate = DateTime.UtcNow;
                await dbContext.SaveChangesAsync();
                await job.LogAsync($"Completed");
            }
            catch (Exception e)
            {
                job.Status = ProcessingJobStatus.Failed;
                await dbContext.SaveChangesAsync();
                await job.LogAsync($"Failed with error: {e.Message}");
                return;
            }
            
            backgroundJobClient.Schedule<DestructJob>(bgJob => bgJob.Execute(job.Id),
                EtherealApplication.DefaultFileLifetime);
        }
    }
}