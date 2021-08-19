using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ethereal.Domain.Entities
{
    public class ProcessingJob
    {
        public Guid Id { get; set; }
        
        public ProcessingJobStatus Status { get; set; }

        public int TotalStepsCount { get; set; }
        
        public int CurrentStepIndex { get; set; }

        public string CurrentStepDescription { get; set; }
        
        public string LocalPath { get; set; }
        
        public virtual ProcessingJobVideo Video { get; set; }

        public class Configuration : IEntityTypeConfiguration<ProcessingJob>
        {
            public void Configure(EntityTypeBuilder<ProcessingJob> builder)
            {
                builder.ToTable("processingJob");
                
                builder.HasKey(j => j.Id);

                builder.Property(j => j.Status)
                    .HasConversion<string>();
                
                builder.HasOne(j => j.Video)
                    .WithOne(j => j.ProcessingJob);
            }
        }
        
    }
}