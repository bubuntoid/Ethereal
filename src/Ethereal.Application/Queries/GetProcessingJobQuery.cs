using System;
using System.Threading.Tasks;
using Ethereal.Application.Exceptions;
using Ethereal.Domain;
using Ethereal.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ethereal.Application.Queries
{
    public class GetProcessingJobQuery
    {
        private readonly EtherealDbContext dbContext;

        public GetProcessingJobQuery(EtherealDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<ProcessingJob> ExecuteAsync(Guid jobId)
        {
            var job = await dbContext.ProcessingJobs
                .Include(j => j.Video)
                .FirstOrDefaultAsync(j => j.Id == jobId);
            
            if (job == null)
                throw new NotFoundException("Job not found");

            return job;
        }
    }
}