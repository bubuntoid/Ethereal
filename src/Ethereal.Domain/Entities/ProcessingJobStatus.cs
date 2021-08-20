namespace Ethereal.Domain.Entities
{
    public enum ProcessingJobStatus
    {
        Failed = -1,
        Created = 0,
        FetchingVideo = 1,
        FetchingThumbnail = 2,
        Processing = 3,
        Archiving = 4,
        Succeed = 100,
        Expired = -100,
    }
}