using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ethereal.Domain.Entities;

public class ProcessingJobVideo
{
    public Guid ProcessingJobId { get; set; }

    public string Url { get; set; }

    public string Id { get; set; }

    public string Title { get; set; }

    public string OriginalDescription { get; set; }

    public string Description { get; set; }

    public TimeSpan Duration { get; set; }

    public virtual ProcessingJob ProcessingJob { get; set; }

    public class Configuration : IEntityTypeConfiguration<ProcessingJobVideo>
    {
        public void Configure(EntityTypeBuilder<ProcessingJobVideo> builder)
        {
            builder.ToTable("processingJobVideo");

            builder.HasKey(v => v.ProcessingJobId);

            builder.HasOne(v => v.ProcessingJob)
                .WithOne(j => j.Video);
        }
    }
}