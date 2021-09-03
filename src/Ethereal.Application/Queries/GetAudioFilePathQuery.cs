using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Ethereal.Application.Exceptions;
using Ethereal.Application.Extensions;
using Ethereal.Domain;
using Ethereal.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using InvalidOperationException = Ethereal.Application.Exceptions.InvalidOperationException;

namespace Ethereal.Application.Queries
{
    public class GetAudioFilePathQuery
    {
        private readonly EtherealDbContext dbContext;

        public GetAudioFilePathQuery(EtherealDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<string> ExecuteAsync(Guid jobId, int index)
        {
            var job = await dbContext.ProcessingJobs
                .Include(j => j.Video)
                .FirstOrDefaultAsync(j => j.Id == jobId);

            if (job == null)
                throw new NotFoundException("Job not found");

            if (job.Status == ProcessingJobStatus.Processing || job.Status == ProcessingJobStatus.Created)
                throw new InvalidOperationException("Video is not processed yet");

            var chapters = job.ParseChapters();
            var chapter = chapters.FirstOrDefault(x => x.Index == index);
            if (chapter == null)
                throw new InvalidOperationException("Chapter not found");

            var path = Path.Combine(job.LocalPath, job.GetChapterLocalFilePath(chapter));

            if (File.Exists(path) == false)
                throw new NotFoundException("File not found");

            return path;
        }
    }
}