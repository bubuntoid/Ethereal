using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ethereal.Application.Exceptions;
using Ethereal.Application.Extensions;
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
        private bool isDownloaded;

        public FetchYoutubeVideoCommand(EtherealDbContext dbContext, YoutubeClient youtubeClient)
        {
            this.dbContext = dbContext;
            this.youtubeClient = youtubeClient;
        }

        public async Task ExecuteAsync(Guid jobId)
        {
            var job = await dbContext.ProcessingJobs
                .Include(j => j.Video)
                .FirstOrDefaultAsync(j => j.Id == jobId);

            if (job == null)
                throw new NotFoundException();

            job.Status = ProcessingJobStatus.FetchingVideo;
            job.CurrentStepDescription = "Fetching video from youtube";
            job.TotalStepsCount = 1;
            job.CurrentStepIndex = 0;
            await dbContext.SaveChangesAsync();

            var cts = new CancellationTokenSource();
            var timeoutDate = DateTime.UtcNow.AddMinutes(2);
            var thread = new Thread(async () =>
            {
                while (true)
                {
                    if (DateTime.UtcNow > timeoutDate)
                    {
                        job.CurrentStepDescription =
                            "Could not fetch video from youtube. It happens sometimes. Try again.";
                        job.Status = ProcessingJobStatus.Failed;
                        // ReSharper disable once MethodSupportsCancellation
                        await dbContext.SaveChangesAsync();
                        cts.Cancel();
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
            var manifestStreams = manifest.GetVideoStreams();
            var videoStreamInfo = manifestStreams.FirstOrDefault(x => x.Container.Name == "mp4");
            await using var videoStream = await youtubeClient.Videos.Streams.GetAsync(videoStreamInfo, cts.Token);
            isDownloaded = true;

            // Saving video locally
            await using var createdFileStream = File.Create(job.GetLocalVideoPath());
            videoStream.Seek(0, SeekOrigin.Begin);
            // ReSharper disable once MethodSupportsCancellation
            await videoStream.CopyToAsync(createdFileStream);

            job.CurrentStepIndex++;
            job.CurrentStepDescription = "Video fetched";
            // ReSharper disable once MethodSupportsCancellation
            await dbContext.SaveChangesAsync();
        }
    }
}