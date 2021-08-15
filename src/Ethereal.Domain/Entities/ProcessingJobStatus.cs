namespace Ethereal.Domain.Entities
{
    public enum ProcessingJobStatus
    {
        Created,
        
        FetchingVideo,
        
        FetchingThumbnail,
        
        Processing,
        
        Completed,
        
        Expired,
        
        Failed,
    }
}