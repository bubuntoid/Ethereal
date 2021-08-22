using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Ethereal.Application.Exceptions;
using Ethereal.Application.Extensions;
using Ethereal.Domain;
using Ethereal.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ethereal.Application.Commands
{
    public class SplitVideoCommand
    {
        private readonly EtherealDbContext dbContext;
        private readonly FfmpegWrapper ffmpegWrapper;

        public SplitVideoCommand(EtherealDbContext dbContext, FfmpegWrapper ffmpegWrapper)
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
            job.Status = ProcessingJobStatus.Processing;
            job.TotalStepsCount = chapters.Count;
            job.CurrentStepIndex = 0;
            await dbContext.SaveChangesAsync();
            
            for (var i = 0; i < chapters.Count; i++)
            {
                var chapter = chapters.ElementAt(i);

                job.CurrentStepIndex++;
                job.CurrentStepDescription = $"Splitting video [{i+1}/{chapters.Count}] ({chapter.Name})";
                await dbContext.SaveChangesAsync();

                await ffmpegWrapper.SaveTrimmedAsync(job.GetLocalVideoPath(), job.GetChapterLocalFilePath(chapter),
                    chapter);
            }

            job.CurrentStepDescription = $"Splitting succeeded";
            await dbContext.SaveChangesAsync();
        }
    }
}