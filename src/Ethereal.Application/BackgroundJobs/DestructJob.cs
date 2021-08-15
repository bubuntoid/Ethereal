using System;
using System.IO;
using System.Threading.Tasks;
using Ethereal.Domain;
using Ethereal.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ethereal.Application.BackgroundJobs
{
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
                // todo: log
                return;
            }

            Directory.Delete(job.LocalPath, true);
            job.Status = ProcessingJobStatus.Expired;
            await dbContext.SaveChangesAsync();
        }
    }
}