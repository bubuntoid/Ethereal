using System;

namespace Ethereal.Domain.Entities
{
    public class ProcessingJobVideo
    {
        public Guid ProcessingJobId { get; set; }
        
        public string Url { get; set; }
        
        public string Id { get; set; }
        
        public string Title { get; set; }
        
        public string OriginalDescription { get; set; }
        
        public string Description { get; set; }
    }
}