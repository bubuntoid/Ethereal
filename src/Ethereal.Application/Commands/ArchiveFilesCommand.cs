using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
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
    public class ArchiveFilesCommand
    {
        private readonly EtherealDbContext dbContext;

        public ArchiveFilesCommand(EtherealDbContext dbContext)
        {
            this.dbContext = dbContext;
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
            
            var zipMemoryStream = new MemoryStream();
            var zipArchive = new ZipArchive(zipMemoryStream, ZipArchiveMode.Create, false);
            for (var i = 0; i < chapters.Count; i++)
            {
                var chapter = chapters.ElementAt(i);
                
                await job.LogAsync($"Archiving files [{i + 1}/{chapters.Count}] ({chapter.Name})");

                var filename = Path.GetFileName(job.GetChapterLocalFilePath(chapter));
                zipArchive.CreateEntryFromFile(job.GetChapterLocalFilePath(chapter),
                    filename.Replace(".mp4", ".mp3"));
            }

            zipArchive.Dispose();
            zipMemoryStream.Close();
            
            await File.WriteAllBytesAsync(job.GetArchivePath(), zipMemoryStream.ToArray());
            
            await job.LogAsync("Archiving completed");
        }
    }
}