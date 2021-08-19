using System;

namespace Ethereal.Domain.Entities
{
    public class ProcessingJob
    {
        public Guid Id { get; set; }
        
        public ProcessingJobStatus Status { get; set; }
        
        public string CurrentProcessingStep { get; set; }
        
        public string LocalPath { get; set; }
        
        public virtual ProcessingJobVideo Video { get; set; }
    }
}