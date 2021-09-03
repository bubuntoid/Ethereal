using System;
using System.Linq;
using System.Threading.Tasks;
using Ethereal.Application.ProcessingJobLogger;
using Ethereal.Domain;
using Ethereal.Domain.Entities;
using Hangfire;
using Microsoft.EntityFrameworkCore;

namespace Ethereal.Application.BackgroundJobs
{
    [AutomaticRetry(Attempts = 0)]
    public class GarbageCleanerJob : BackgroundJobBase<TimeSpan>
    {
        private readonly EtherealDbContext dbContext;
        private readonly DestructJob destructJob;

        public GarbageCleanerJob(EtherealDbContext dbContext, DestructJob destructJob)
        {
            this.dbContext = dbContext;
            this.destructJob = destructJob;
        }
        
        public override async Task ExecuteAsync(TimeSpan timeout)
        {
            var jobs = await dbContext.ProcessingJobs
                .Where(j => j.Status == ProcessingJobStatus.Processing)
                .Take(100)
                .ToListAsync();

            foreach (var job in jobs)
            {
                if (job.CreatedDate < DateTime.UtcNow - timeout)
                {
                    job.Status = ProcessingJobStatus.Failed;
                    await dbContext.SaveChangesAsync();
                    await job.LogAsync("Job failed due to long processing status");
                    await destructJob.ExecuteAsync(job.Id);
                }
            }
        }
    }
}