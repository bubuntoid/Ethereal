using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Ethereal.Application.Exceptions;
using Ethereal.Application.Extensions;
using Ethereal.Application.ProcessingJobLogger;
using Ethereal.Domain;
using Ethereal.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ethereal.Application.Commands
{
    public class FetchThumbnailsCommand
    {
        private readonly EtherealDbContext dbContext;
        private readonly FfmpegWrapper ffmpegWrapper;

        public FetchThumbnailsCommand(EtherealDbContext dbContext, FfmpegWrapper ffmpegWrapper)
        {
            this.dbContext = dbContext;
            this.ffmpegWrapper = ffmpegWrapper;
        }
        
        public async Task ExecuteAsync(Guid jobId)
        {
            var job = await dbContext.ProcessingJobs
                .Include(j => j.Video)
                .FirstOrDefaultAsync(j => j.Id == jobId);
            
            if (job == null)
                throw new NotFoundException();
            
            var chapters = job.ParseChapters();
            job.Status = ProcessingJobStatus.FetchingThumbnail;
            job.TotalStepsCount = chapters.Count;
            job.CurrentStepIndex = 0;
            await dbContext.SaveChangesAsync();
            await job.LogAsync("Fetching thumbnails...");
            
            var directory = job.GetLocalThumbnailsDirectoryPath();
            Directory.CreateDirectory(directory);            
            
            for (var i = 0; i < chapters.Count; i++)
            {
                var chapter = chapters.ElementAt(i);

                var currentStepDescription = $"Fetching thumbnails [{i + 1}/{chapters.Count}] ({chapter.Name})"; 
                job.CurrentStepIndex++;
                job.CurrentStepDescription = currentStepDescription;
                await dbContext.SaveChangesAsync();
                await job.LogAsync(currentStepDescription);
                
                var path = Path.Combine(directory, $"{i}.png");
                await ffmpegWrapper.SaveImageAsync(job.GetLocalVideoPath(), path, chapter);
            }
            
            job.CurrentStepDescription = $"Thumbnails fetched";
            await dbContext.SaveChangesAsync();
            await job.LogAsync("Thumbnails fetched");
        }
    }
}