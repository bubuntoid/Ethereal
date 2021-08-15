namespace Ethereal.Domain.Entities
{
    public enum ProcessingJobStatus
    {
        Created,
        
        FetchingVideo,
        
        FetchingThumbnail,
        
        Processing,
        
        Archiving,
        
        Completed,
        
        Expired,
        
        Failed,
    }
}