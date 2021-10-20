using System;
using System.IO;
using System.Threading.Tasks;
using Ethereal.Application.ProcessingJobLogger;
using Ethereal.Domain;
using Ethereal.Domain.Entities;
using Hangfire;
using Microsoft.EntityFrameworkCore;

namespace Ethereal.Application.BackgroundJobs
{
    [AutomaticRetry(Attempts = 0)]
    public class DestructJob : BackgroundJobBase<Guid>
    {
        private readonly EtherealDbContext dbContext;

        public DestructJob(EtherealDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        
        public override async Task ExecuteAsync(Guid jobId)
        {
            var job = await dbContext.ProcessingJobs.FirstOrDefaultAsync(j => j.Id == jobId);
            if (job == null)
            {
                return;
            }

            try
            {
                Directory.Delete(job.LocalPath, true);
                await job.LogAsync($"Cache deleted");
            }
            catch (Exception e)
            {
                await job.LogAsync($"Could not delete files: {e.Message}");
            }
            finally
            {
                job.Status = ProcessingJobStatus.Expired;
                await dbContext.SaveChangesAsync();
                await job.LogAsync($"Job expired");
            }
        }
    }
}