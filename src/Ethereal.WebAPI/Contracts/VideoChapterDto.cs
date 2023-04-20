namespace Ethereal.WebAPI.Contracts;

public class VideoChapterDto
{
    public string Original { get; set; }

    public int Index { get; set; }

    public string Name { get; set; }

    public string StartTimespan { get; set; }

    public string EndTimespan { get; set; }

    public string Duration { get; set; }

    public string Mp3Url { get; set; }

    public string ThumbnailUrl { get; set; }
}