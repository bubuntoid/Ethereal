using System;

namespace YOVPS.Core
{
    public class VideoChapter
    {
        public string Original { get; set; }

        public string ParsedName { get; set; }

        public TimeSpan ParsedTimespan { get; set; }

        public VideoChapter()
        {

        }

        public VideoChapter(string original, string name, TimeSpan timespan)
        {
            Original = original;
            ParsedName = name;
            ParsedTimespan = timespan;
        }
    }
}
