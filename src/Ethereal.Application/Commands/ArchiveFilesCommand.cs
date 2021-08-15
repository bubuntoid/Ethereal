using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Ethereal.Application.Extensions;
using Ethereal.Domain;
using Ethereal.Domain.Entities;

namespace Ethereal.Application.Commands
{
    public class ArchiveFilesCommand
    {
        private readonly EtherealDbContext dbContext;

        public ArchiveFilesCommand(EtherealDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task ExecuteAsync(ProcessingJob job, IReadOnlyCollection<VideoChapter> chapters)
        {
            job.Status = ProcessingJobStatus.Archiving;
            await dbContext.SaveChangesAsync();
            
            var zipMemoryStream = new MemoryStream();
            var zipArchive = new ZipArchive(zipMemoryStream, ZipArchiveMode.Create, false);
            for (var i = 0; i < chapters.Count; i++)
            {
                var chapter = chapters.ElementAt(i);

                job.CurrentProcessingStep = $"Archiving files [{i}/{chapters.Count}] ({chapter.Name})";
                await dbContext.SaveChangesAsync();

                var filename = Path.GetFileName(job.GetChapterLocalFilePath(chapter));
                zipArchive.CreateEntryFromFile(job.GetChapterLocalFilePath(chapter),
                    filename.Replace(".mp4", ".mp3"));
            }

            zipArchive.Dispose();
            zipMemoryStream.Close();
          
            job.Status = ProcessingJobStatus.Completed;
            job.CurrentProcessingStep = $"Archiving completed";
            await dbContext.SaveChangesAsync();
        }
    }
}