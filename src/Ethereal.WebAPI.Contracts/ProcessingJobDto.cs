using System;
using Ethereal.Domain.Entities;

namespace Ethereal.WebAPI.Contracts
{
    public class ProcessingJobDto
    {
        public Guid Id { get; set; }
        
        public ProcessingJobStatus Status { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ProcessedDate { get; set; }
        
        public ProcessingJobVideoDto Video { get; set; }
    }
}