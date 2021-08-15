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

        public FetchThumbnailsCommand(EtherealDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        
        public async Task ExecuteAsync(ProcessingJob job, IReadOnlyCollection<VideoChapter> chapters)
        {
            job.Status = ProcessingJobStatus.FetchingThumbnail;
            await dbContext.SaveChangesAsync();
            
            for (var i = 0; i < chapters.Count; i++)
            {
                var chapter = chapters.ElementAt(i);

                job.CurrentProcessingStep = $"Fetching thumbnails [{i}/{chapters.Count}] ({chapter.Name})";
                await dbContext.SaveChangesAsync();
                
                var path = Path.Combine(job.GetLocalThumbnailsDirectoryPath(), $"{i}.png");
                await FfmpegWrapper.SaveImageAsync(job.GetLocalVideoPath(), path, chapter);
            }
            
            job.CurrentProcessingStep = $"Thumbnails fetched";
            await dbContext.SaveChangesAsync();
        }
    }
}