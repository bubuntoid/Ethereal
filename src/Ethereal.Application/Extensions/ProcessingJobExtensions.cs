using System.Collections.Generic;
using System.IO;
using Ethereal.Domain.Entities;

namespace Ethereal.Application.Extensions
{
    public static class ProcessingJobExtensions
    {
        public static string GetLocalVideoPath(this ProcessingJob job, IEtherealSettings settings)
        {
            return Path.Combine(job.LocalPath, settings.OriginalVideoFileName);
        }

        public static string GetLocalThumbnailsDirectoryPath(this ProcessingJob job, IEtherealSettings settings)
        {
            return Path.Combine(job.LocalPath, settings.ThumbnailsDirectoryName);
        }

        public static string GetArchivePath(this ProcessingJob job)
        {
            return Path.Combine(job.LocalPath, $"{job.Video.Title.RemoveIllegalCharacters()}.zip");
        }

        public static string GetChapterLocalFilePath(this ProcessingJob job, VideoChapter chapter)
        {
            return Path.Combine(job.LocalPath, $"{chapter.Name.RemoveIllegalCharacters()}.mp4");
        }

        public static string GetChapterLocalThumbnailFilePath(this ProcessingJob job, VideoChapter chapter, IEtherealSettings settings)
        {
            return Path.Combine(GetLocalThumbnailsDirectoryPath(job, settings), $"{chapter.Index}.png");
        }
        
        public static IReadOnlyCollection<VideoChapter> ParseChapters(this ProcessingJob job)
        {
            var chapters = new VideoDescription(job.Video.Description).ParseChapters();
            foreach (var item in chapters)
            {
                item.EndTimespan ??= job.Video.Duration;
            }
            return chapters;
        }

        public static string GetLogFilePath(this ProcessingJob job, IEtherealSettings settings)
        {
            return Path.Combine(settings.TempPath, settings.LogsDirectoryName, $"{job.Id}.txt");
        }
    }
}