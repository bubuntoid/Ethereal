using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ethereal.Application.Extensions;
using Ethereal.Domain.Entities;
using YoutubeExplode;
using YoutubeExtractor;

namespace Ethereal.Application.YouTube
{
    public class YoutubeExtractorProvider : IYoutubeProvider
    {
        public async Task<YoutubeVideo> GetVideoAsync(string url)
        {
            var video = await new YoutubeClient().Videos.GetAsync(url);
            return new YoutubeVideo
            {
                Description = video.Description,
                Id = video.Id,
                Duration = video.Duration,
                Title = video.Title,
            };
        }

        public Task DownloadAsync(ProcessingJob job, CancellationTokenSource cts)
        {
            var videos = DownloadUrlResolver.GetDownloadUrls(job.Video.Url);
            var video = videos.First(p => p.VideoType == VideoType.Mp4);
            if (video.RequiresDecryption)
                DownloadUrlResolver.DecryptDownloadUrl(video);
            var downloader = new VideoDownloader(video, job.GetLocalVideoPath());
            downloader.DownloadProgressChanged += (sender, args) =>
            {
                var message = $"Fetching video... [{args.ProgressPercentage:P0}]";
                ProcessingJobLogger.ProcessingJobLogger.InternalLogger.Info(message);
                ProcessingJobLogger.ProcessingJobLogger.OnLog(job, message);
            };
            downloader.Execute();
            return Task.CompletedTask;
        }
    }
}   