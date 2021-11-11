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
    public class ConvertVideoCommand
    {
        private readonly EtherealDbContext dbContext;
        private readonly FfmpegWrapper ffmpegWrapper;
        private readonly IEtherealSettings settings;

        public ConvertVideoCommand(EtherealDbContext dbContext, FfmpegWrapper ffmpegWrapper, IEtherealSettings settings)
        {
            this.dbContext = dbContext;
            this.ffmpegWrapper = ffmpegWrapper;
            this.settings = settings;
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
            await dbContext.SaveChangesAsync();
            
            for (var i = 0; i < chapters.Count; i++)
            {
                var chapter = chapters.ElementAt(i);

                await job.LogAsync($"Converting video [{i + 1}/{chapters.Count}] ({chapter.Name})");

                await ffmpegWrapper.SaveTrimmedAsync(job.GetLocalVideoPath(settings), job.GetChapterLocalFilePath(chapter),
                    chapter);
            }

            await dbContext.SaveChangesAsync();
            await job.LogAsync("Converting succeeded");
        }
    }
}