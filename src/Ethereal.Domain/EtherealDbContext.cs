using Ethereal.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ethereal.Domain
{
    public class EtherealDbContext : DbContext
    {
        public DbSet<ProcessingJob> ProcessingJobs { get; set; }
    }
}