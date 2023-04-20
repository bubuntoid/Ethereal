using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ethereal.Domain.Entities;

public class ProcessingJob
{
    public Guid Id { get; set; }

    public ProcessingJobStatus Status { get; set; }

    public string LocalPath { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? ProcessedDate { get; set; }

    public virtual ProcessingJobVideo Video { get; set; }

    public string LastLogMessage { get; set; }

    public class Configuration : IEntityTypeConfiguration<ProcessingJob>
    {
        public void Configure(EntityTypeBuilder<ProcessingJob> builder)
        {
            builder.ToTable("processingJob");

            builder.HasKey(j => j.Id);

            builder.Property(j => j.Status)
                .HasConversion<string>();

            builder.HasOne(j => j.Video)
                .WithOne(v => v.ProcessingJob);
        }
    }
}