using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ethereal.Application.Extensions;
using Ethereal.Application.ProcessingJobLogger;
using Ethereal.Domain.Entities;
using YoutubeExplode;

namespace Ethereal.Application.YouTube
{
    public class YoutubeExplodeProvider : IYoutubeProvider // todo: add tests
    {
        private readonly YoutubeClient client;
        
        public YoutubeExplodeProvider()
        {
            client = new YoutubeClient();
        }
        
        public async Task<YoutubeVideo> GetVideoAsync(string url)
        {
            var video = await client.Videos.GetAsync(url);
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
            // Fetching video from youtube
            var manifest = await client.Videos.Streams.GetManifestAsync(job.Video.Id, cts.Token);
            await job.LogAsync("Manifest loaded");
            var manifestStreams = manifest.GetVideoStreams();
            var videoStreamInfo = manifestStreams.First(x => x.Container.Name == "mp4");
            await job.LogAsync("Video stream info found");

            // Saving video locally
            await job.LogAsync("Fetching video...");
            var progress = new Progress<double>(p =>
            {
                var message = $"Fetching video... [{p:P0}]";
                ProcessingJobLogger.ProcessingJobLogger.InternalLogger.Info(message);
                ProcessingJobLogger.ProcessingJobLogger.OnLog(job, message);
            });
            await client.Videos.Streams.DownloadAsync(videoStreamInfo, job.GetLocalVideoPath(), progress, cts.Token);
            await job.LogAsync("Video fetched");
        }
    }
}