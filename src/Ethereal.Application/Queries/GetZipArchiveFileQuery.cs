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
    public class GetZipArchiveFileQuery
    {
        private readonly EtherealDbContext dbContext;

        public GetZipArchiveFileQuery(EtherealDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<string> ExecuteAsync(Guid jobId)
        {
            var job = await dbContext.ProcessingJobs
                .Include(j => j.Video)
                .FirstOrDefaultAsync(j => j.Id == jobId);

            if (job == null)
                throw new Exception("Job not found");
            
            if (job.Status != ProcessingJobStatus.Completed)
                throw new Exception("Video is not processed");
            
            var path = Path.Combine(job.LocalPath, job.GetArchivePath());

            if (File.Exists(path) == false)
                throw new Exception("File not found");

            return path;
        }
    }
}