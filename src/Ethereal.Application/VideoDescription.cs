using System;
using System.Collections.Generic;
using System.Linq;
using Ethereal.Application.Exceptions;
using Ethereal.Application.Extensions;

namespace Ethereal.Application
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
            if (Description == null)
                throw new InternalErrorException(message: "Could not parse any chapter");
            
            var chapters = new List<VideoChapter>();

            var lines = Description.Split("\n").ToList(); 
            lines = lines.Count == 1 ? Description.Split(@"\n").ToList() : lines;
            
            var firstLine = lines.FirstOrDefault(line => FirstChapterVariants.Any(line.Contains));
            var index = lines.IndexOf(firstLine);

            if (index == -1 || lines.Count == 0)
                throw new InternalErrorException(message: "Could not parse any chapter");
            
            var chapterIndex = -1;
            while (lines.Count > index)
            {
                var line = lines[index].Trim();
                index++;

                if (line.ContainsTimespan(out var timespan) == false)
                    continue;

                var chapter = new VideoChapter
                {
                    Index = ++chapterIndex,
                    Original = line,
                    StartTimespan = timespan,
                    Name = line.RemoveTimespan().RemoveIllegalCharacters(),
                };
                chapters.Add(chapter);
            }

            for (var i = 0; i < chapters.Count; i++)
            {
                var currentChapter = chapters.ElementAt(i);
                currentChapter.EndTimespan = i == chapters.Count - 1
                    // ReSharper disable once PossibleInvalidOperationException
                    ? (TimeSpan?) null
                    : chapters.ElementAt(i + 1).StartTimespan;
            }

            return chapters;
        }
    }
}
