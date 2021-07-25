using System;

namespace YOVPS.Core
{
    public class VideoChapter
    {
        public string Original { get; set; }

        public string Name { get; set; }

        public TimeSpan StartTimespan { get; set; }
        
        public TimeSpan? EndTimespan { get; set; }

        public TimeSpan? Duration => EndTimespan - StartTimespan;
        
        public VideoChapter()
        {

        }

        public VideoChapter(string original, string name, TimeSpan timespan)
        {
            Original = original;
            Name = name;
            StartTimespan = timespan;
        }
    }
}
