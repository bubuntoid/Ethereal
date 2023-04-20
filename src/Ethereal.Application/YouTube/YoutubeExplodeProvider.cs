using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ethereal.Application.Extensions;
using Ethereal.Application.ProcessingJobLogger;
using Ethereal.Domain.Entities;
using YoutubeExplode;

namespace Ethereal.Application.YouTube;

[Obsolete("Slow downloading speed")]
public class YoutubeExplodeProvider : IYoutubeProvider
{
    private readonly YoutubeClient client;
    private readonly IEtherealSettings settings;

    public YoutubeExplodeProvider(IEtherealSettings settings)
    {
        this.settings = settings;
        client = new YoutubeClient();
    }

    public async Task<YoutubeVideo> GetVideoAsync(string url)
    {
        // todo: dry
        var video = await client.Videos.GetAsync(url);
        return new YoutubeVideo
        {
            Description = video.Description,
            Id = video.Id,
            Duration = video.Duration,
            Title = video.Title
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
        await client.Videos.Streams.DownloadAsync(videoStreamInfo, job.GetLocalVideoPath(settings), progress, cts.Token);
        await job.LogAsync("Video fetched");
    }
}