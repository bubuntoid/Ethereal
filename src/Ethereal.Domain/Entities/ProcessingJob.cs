using System;

namespace Ethereal.Domain.Entities
{
    public class ProcessingJob
    {
        public Guid Id { get; set; }
        
        public string VideoUrl { get; set; }
        
        public string VideoId { get; set; }
        
        public string VideoTitle { get; set; }
        
        public string VideoDescription { get; set; }
        
        public ProcessingJobStatus Status { get; set; }
        
        public string CurrentProcessingStep { get; set; }
        
        public string LocalPath { get; set; }
    }
}