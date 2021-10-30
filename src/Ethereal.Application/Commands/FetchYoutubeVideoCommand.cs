using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ethereal.Application.Exceptions;
using Ethereal.Application.Extensions;
using Ethereal.Application.ProcessingJobLogger;
using Ethereal.Application.YouTube;
using Ethereal.Domain;
using Ethereal.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ethereal.Application.Commands
{
    public class FetchYoutubeVideoCommand
    {
        private readonly EtherealDbContext dbContext;
        private readonly IYoutubeProvider youtubeProvider;
        private readonly IEtherealSettings settings;
        private readonly IDatabaseSettings databaseSettings;
        private bool isDownloaded;

        public FetchYoutubeVideoCommand(EtherealDbContext dbContext, IYoutubeProvider youtubeProvider,
            IEtherealSettings settings, IDatabaseSettings databaseSettings)
        {
            this.dbContext = dbContext;
            this.youtubeProvider = youtubeProvider;
            this.settings = settings;
            this.databaseSettings = databaseSettings;
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
            await job.LogAsync("Fetching video from youtube...");

            var cts = new CancellationTokenSource();
            var timeoutDate = DateTime.UtcNow.Add(settings.DownloadingTimeout);
            var thread = new Thread(async () =>
            {
                while (true)
                {
                    // Console.WriteLine(timeoutDate.ToLongTimeString() + " || " + DateTime.UtcNow.ToLongTimeString());
                    
                    if (DateTime.UtcNow > timeoutDate)
                    {
                        // ReSharper disable once InconsistentNaming
                        await using var _dbContext = new EtherealDbContext(databaseSettings);
                        // ReSharper disable once MethodSupportsCancellation
                        // ReSharper disable once InconsistentNaming
                        var _job = await _dbContext.ProcessingJobs.FirstOrDefaultAsync(j => j.Id == job.Id);
                        _job.Status = ProcessingJobStatus.Failed;
                        await _job.LogAsync("Could not fetch video from youtube. It happens sometimes. Try again.");

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

            await youtubeProvider.DownloadAsync(job, cts);
            isDownloaded = true;
            await job.LogAsync("Video fetched successfully");
        }
    }
}