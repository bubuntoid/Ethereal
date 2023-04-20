namespace Ethereal.WebAPI.Contracts;

public class ProcessingJobVideoDto
{
    public string Url { get; set; }

    public string Id { get; set; }

    public string Title { get; set; }

    public string OriginalDescription { get; set; }

    public string Description { get; set; }

    public string Duration { get; set; }
}