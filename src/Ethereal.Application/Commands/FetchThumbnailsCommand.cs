using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Ethereal.Application.Extensions;
using Ethereal.Domain;
using Ethereal.Domain.Entities;

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
        
        public async Task ExecuteAsync(ProcessingJob job, IReadOnlyCollection<VideoChapter> chapters)
        {
            job.Status = ProcessingJobStatus.FetchingThumbnail;
            job.TotalStepsCount = chapters.Count;
            await dbContext.SaveChangesAsync();
            
            for (var i = 0; i < chapters.Count; i++)
            {
                var chapter = chapters.ElementAt(i);

                job.CurrentStepIndex++;
                job.CurrentStepDescription = $"Fetching thumbnails [{i}/{chapters.Count}] ({chapter.Name})";
                await dbContext.SaveChangesAsync();
                
                var path = Path.Combine(job.GetLocalThumbnailsDirectoryPath(), $"{i}.png");
                await ffmpegWrapper.SaveImageAsync(job.GetLocalVideoPath(), path, chapter);
            }
            
            job.CurrentStepDescription = $"Thumbnails fetched";
            await dbContext.SaveChangesAsync();
        }
    }
}