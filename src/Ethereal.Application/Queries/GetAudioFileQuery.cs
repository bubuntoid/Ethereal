using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Ethereal.Application.Extensions;
using Ethereal.Domain;
using Ethereal.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ethereal.Application.Queries
{
    public class GetAudioFileQuery
    {
        private readonly EtherealDbContext dbContext;

        public GetAudioFileQuery(EtherealDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<string> ExecuteAsync(Guid jobId, int index)
        {
            var job = await dbContext.ProcessingJobs
                .Include(j => j.Video)
                .FirstOrDefaultAsync(j => j.Id == jobId);

            if (job == null)
                throw new Exception("Job not found");

            if (job.Status < ProcessingJobStatus.Processing)
                throw new Exception("Video is not processed yet");

            if (job.Status == ProcessingJobStatus.Processing && index <= job.CurrentStepIndex)
                throw new Exception("Video is not processed yet");

            var chapters = job.ParseChapters();
            var chapter = chapters.FirstOrDefault(x => x.Index == index);
            if (chapter == null)
                throw new Exception("Chapter not found");

            var path = Path.Combine(job.LocalPath, job.GetChapterLocalFilePath(chapter));

            if (File.Exists(path) == false)
                throw new Exception("File not found");

            return path;
        }
    }
}