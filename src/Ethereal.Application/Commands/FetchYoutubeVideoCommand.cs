using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ethereal.Application.Exceptions;
using Ethereal.Application.Extensions;
using Ethereal.Application.ProcessingJobLogger;
using Ethereal.Domain;
using Ethereal.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using YoutubeExplode;

namespace Ethereal.Application.Commands
{
    public class FetchYoutubeVideoCommand
    {
        private readonly EtherealDbContext dbContext;
        private readonly YoutubeClient youtubeClient;
        private readonly IEtherealSettings settings;
        private bool isDownloaded;

        public FetchYoutubeVideoCommand(EtherealDbContext dbContext, YoutubeClient youtubeClient,
            IEtherealSettings settings)
        {
            this.dbContext = dbContext;
            this.youtubeClient = youtubeClient;
            this.settings = settings;
        }

        public async Task ExecuteAsync(Guid jobId)
        {
            var job = await dbContext.ProcessingJobs
                .Include(j => j.Video)
                .FirstOrDefaultAsync(j => j.Id == jobId);

            if (job == null)
                throw new NotFoundException();

            job.Status = ProcessingJobStatus.Processing;
            await dbContext.SaveChangesAsync();
            await job.LogAsync(dbContext, "Fetching video from youtube...");

            var cts = new CancellationTokenSource();
            var timeoutDate = DateTime.UtcNow.Add(settings.DownloadingTimeout);
            var thread = new Thread(async () =>
            {
                while (true)
                {
                    // Console.WriteLine(timeoutDate.ToLongTimeString() + " || " + DateTime.UtcNow.ToLongTimeString());
                    
                    if (DateTime.UtcNow > timeoutDate)
                    {
                        job.Status = ProcessingJobStatus.Failed;
                        // ReSharper disable once MethodSupportsCancellation
                        await dbContext.SaveChangesAsync();
                        await job.LogAsync(dbContext, "Could not fetch video from youtube. It happens sometimes. Try again.");

                        cts.Cancel();
                        // throw new InternalErrorException(
                        //     "Could not fetch video from youtube. It happens sometimes. Try again.");
                        return;
                    }

                    if (isDownloaded)
                        return;

                    // ReSharper disable once MethodSupportsCancellation
                    await Task.Delay(10);
                }
            });
            thread.Start();

            // Fetching video from youtube
            var manifest = await youtubeClient.Videos.Streams.GetManifestAsync(job.Video.Id, cts.Token);
            await job.LogAsync(dbContext, "Manifest loaded");
            var manifestStreams = manifest.GetVideoStreams();
            var videoStreamInfo = manifestStreams.First(x => x.Container.Name == "mp4");
            await job.LogAsync(dbContext, "Video stream info found");

            // Saving video locally
            await job.LogAsync(dbContext, "Fetching video...");
            var progress = new Progress<double>(p =>
                ProcessingJobLogger.ProcessingJobLogger.OnLog(job, $"Fetching video... [{p:P0}]"));
            await youtubeClient.Videos.Streams.DownloadAsync(videoStreamInfo, job.GetLocalVideoPath(), progress, cts.Token);
            await job.LogAsync(dbContext, "Video fetched");
            isDownloaded = true;
        }
    }
}