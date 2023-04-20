using System;

namespace Ethereal.Application.YouTube;

public class YoutubeVideo
{
    public string Id { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public TimeSpan? Duration { get; set; }
}