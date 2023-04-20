using System;

namespace Ethereal.Application;

public class VideoChapter
{
    public VideoChapter()
    {
    }

    public VideoChapter(string original, string name, TimeSpan timespan)
    {
        Original = original;
        Name = name;
        StartTimespan = timespan;
    }

    public string Original { get; set; }

    public int Index { get; set; }

    public string Name { get; set; }

    public TimeSpan StartTimespan { get; set; }

    public TimeSpan? EndTimespan { get; set; }

    public TimeSpan? Duration => EndTimespan - StartTimespan;
}