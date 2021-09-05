using System;
using System.Collections.Generic;
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
        
        public IReadOnlyCollection<VideoChapterDto> Chapters { get; set; }
        
        public string ZipArchiveUrl { get; set; }
    }
}