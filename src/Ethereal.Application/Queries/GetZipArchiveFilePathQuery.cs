using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Ethereal.Application.Exceptions;
using Ethereal.Application.Extensions;
using Ethereal.Domain;
using Ethereal.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using InvalidOperationException = System.InvalidOperationException;

namespace Ethereal.Application.Queries
{
    public class GetZipArchiveFilePathQuery
    {
        private readonly EtherealDbContext dbContext;

        public GetZipArchiveFilePathQuery(EtherealDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<string> ExecuteAsync(Guid jobId)
        {
            var job = await dbContext.ProcessingJobs
                .Include(j => j.Video)
                .FirstOrDefaultAsync(j => j.Id == jobId);

            if (job == null)
                throw new NotFoundException("Job not found");
            
            if (job.Status == ProcessingJobStatus.Processing || job.Status == ProcessingJobStatus.Created)
                throw new InvalidOperationException("Video is not processed yet");

            var path = job.GetArchivePath();
            
            if (File.Exists(path) == false)
                throw new NotFoundException("Zip ar not found");

            return path;
        }
    }
}