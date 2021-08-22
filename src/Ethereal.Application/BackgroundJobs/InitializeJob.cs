using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Ethereal.Application.Commands;
using Ethereal.Application.Extensions;
using Ethereal.Domain;
using Ethereal.Domain.Entities;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using YoutubeExplode;

namespace Ethereal.Application.BackgroundJobs
{
    public class InitializeJob : BackgroundJobBase<Guid>
    {
        private readonly EtherealDbContext dbContext;
        private readonly FetchThumbnailsCommand fetchThumbnailsCommand;
        private readonly FetchYoutubeVideoCommand fetchYoutubeVideoCommand;
        private readonly SplitVideoCommand splitVideoCommand;
        private readonly IBackgroundJobClient backgroundJobClient;
        private readonly ArchiveFilesCommand archiveFilesCommand;

        public InitializeJob(EtherealDbContext dbContext,
            FetchThumbnailsCommand fetchThumbnailsCommand, FetchYoutubeVideoCommand fetchYoutubeVideoCommand,
            SplitVideoCommand splitVideoCommand, IBackgroundJobClient backgroundJobClient,
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
            {
                // todo: log
                return;
            }

            var chapters = new VideoDescription(job.Video.Description).ParseChapters();
            if (chapters?.Any() == false)
                throw new Exception();

            try
            {
                await fetchYoutubeVideoCommand.ExecuteAsync(job.Id);
                await fetchThumbnailsCommand.ExecuteAsync(job.Id);
                await splitVideoCommand.ExecuteAsync(job.Id);
                await archiveFilesCommand.ExecuteAsync(job.Id);

                job.Status = ProcessingJobStatus.Succeed;
                job.CurrentStepIndex = job.TotalStepsCount = 1;
                job.CurrentStepDescription = "Completed"; // todo: log
                job.ProcessedDate = DateTime.UtcNow;
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                // todo: log
                job.Status = ProcessingJobStatus.Failed;
                await dbContext.SaveChangesAsync();
                return;
            }
            
            backgroundJobClient.Schedule<DestructJob>(bgJob => bgJob.Execute(job.Id),
                EtherealApplication.DefaultFileLifetime);
        }
    }
}