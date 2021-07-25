using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using YOVPS.Core.Extensions;

namespace YOVPS.Core
{
    public class VideoDescription
    {
        // ReSharper disable once InconsistentNaming
        private readonly string[] FirstChapterVariants = {
            "0:00", 
            "00:00", 
            "00:00:00"
        };

        public string Description { get; }

        public VideoDescription(string description)
        {
            Description = description;
        }

        public IReadOnlyCollection<VideoChapter> ParseChapters()
        {
            var chapters = new List<VideoChapter>();

            var lines = Description.Split("\n").ToList(); 
            lines = lines.Count == 1 ? Description.Split(@"\n").ToList() : lines;
            
            var firstLine = lines.FirstOrDefault(line => FirstChapterVariants.Any(line.Contains));
            var index = lines.IndexOf(firstLine);

            while(lines.Count > index)
            {
                var line = lines[index].Trim();
                if (line.ContainsTimespan(out var timespan) == false)
                    break;

                var chapter = new VideoChapter
                {
                    Original = line,
                    StartTimespan = timespan,
                    Name = line.RemoveTimespan().RemoveIllegalCharacters(),
                    
                };
                chapters.Add(chapter);

                index++;
            }

            for (var i = 0; i < chapters.Count; i++)
            {
                var currentChapter = chapters.ElementAt(i);
                currentChapter.EndTimespan = i == chapters.Count - 1
                    // ReSharper disable once PossibleInvalidOperationException
                    ? null
                    : chapters.ElementAt(i + 1).StartTimespan;
            }

            return chapters;
        }
    }
}
