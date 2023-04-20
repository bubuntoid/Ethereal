namespace Ethereal.Domain.Entities;

public enum ProcessingJobStatus
{
    Created,
    Processing,
    Succeed,
    Expired,
    Failed
}