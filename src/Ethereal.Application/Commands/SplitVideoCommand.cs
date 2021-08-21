using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Ethereal.Application.Extensions;
using Ethereal.Domain;
using Ethereal.Domain.Entities;

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
        
        public async Task ExecuteAsync(ProcessingJob job, IReadOnlyCollection<VideoChapter> chapters)
        {
            job.Status = ProcessingJobStatus.Processing;
            job.TotalStepsCount = chapters.Count;
            await dbContext.SaveChangesAsync();
            
            for (var i = 0; i < chapters.Count; i++)
            {
                var chapter = chapters.ElementAt(i);

                job.CurrentStepIndex++;
                job.CurrentStepDescription = $"Splitting video [{i}/{chapters.Count}] ({chapter.Name})";
                await dbContext.SaveChangesAsync();

                await ffmpegWrapper.SaveTrimmedAsync(job.GetLocalVideoPath(), job.GetChapterLocalFilePath(chapter),
                    chapter);
            }

            job.CurrentStepDescription = $"Splitting succeeded";
            await dbContext.SaveChangesAsync();
        }
    }
}