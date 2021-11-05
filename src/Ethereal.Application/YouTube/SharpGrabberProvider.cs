using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using DotNetTools.SharpGrabber;
using DotNetTools.SharpGrabber.Grabbed;
using Ethereal.Application.Exceptions;
using Ethereal.Application.Extensions;
using Ethereal.Application.ProcessingJobLogger;
using Ethereal.Application.Utility;
using Ethereal.Domain.Entities;
using YoutubeExplode;

namespace Ethereal.Application.YouTube
{
    public class SharpGrabberProvider : IYoutubeProvider
    {
        public async Task<YoutubeVideo> GetVideoAsync(string url)
        {
            // todo: dry
            var video = await new YoutubeClient().Videos.GetAsync(url);
            return new YoutubeVideo
            {
                Description = video.Description,
                Id = video.Id,
                Duration = video.Duration,
                Title = video.Title,
            };
        }

        public async Task DownloadAsync(ProcessingJob job, CancellationTokenSource cts)
        {
            var grabber = GrabberBuilder.New()
                .UseDefaultServices()
                .AddYouTube()
                .Build();
            
            var grabResult = await grabber.GrabAsync(new Uri(job.Video.Url));
            var media = grabResult.Resources<GrabbedMedia>().FirstOrDefault(s => s.FormatTitle.Contains("MP4 360p"));
            if (media == null)
            {
                throw new InternalErrorException($"Could not find grabbed media for this video ({job.Video.Url})");
            }
            
            using var client = new HttpClientDownloadWithProgress(media.ResourceUri.ToString(), job.GetLocalVideoPath());
            await job.LogAsync($"Downloading {media.Title ?? media.FormatTitle ?? media.Resolution}...");
            client.ProgressChanged += (totalFileSize, totalBytesDownloaded, progressPercentage) =>
            {
                var message = $"Fetching video... [{progressPercentage} ({totalBytesDownloaded}/{totalFileSize}) ]";
                ProcessingJobLogger.ProcessingJobLogger.InternalLogger.Info(message);
                ProcessingJobLogger.ProcessingJobLogger.OnLog(job, message);
            };

            await client.StartDownload();
        }
    }
}