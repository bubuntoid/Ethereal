using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ethereal.Application.Exceptions;
using Ethereal.Application.Extensions;
using Ethereal.Application.YouTube;
using Ethereal.Domain.Entities;

namespace Ethereal.Application.Queries;

public class GetVideoChaptersPreviewQuery
{
    private readonly IEtherealSettings settings;
    private readonly IYoutubeProvider youtubeProvider;

    public GetVideoChaptersPreviewQuery(IEtherealSettings settings, IYoutubeProvider youtubeProvider)
    {
        this.settings = settings;
        this.youtubeProvider = youtubeProvider;
    }

    public async Task<IReadOnlyCollection<VideoChapter>> ExecuteAsync(string url, string description = null)
    {
        var youtubeVideo = await youtubeProvider.GetVideoAsync(url);
        var desc = description ?? youtubeVideo.Description;

        if (youtubeVideo.Duration.HasValue == false || youtubeVideo.Duration.Value == TimeSpan.Zero)
            throw new InternalErrorException("Live streams not supported");

        if (youtubeVideo.Duration.Value > settings.VideoDurationLimit)
            throw new InternalErrorException($"Video duration exceeded time limit ({settings.VideoDurationLimit})");

        var job = new ProcessingJob
        {
            Id = Guid.NewGuid(),
            Status = ProcessingJobStatus.Created,
            CreatedDate = DateTime.UtcNow,

            Video = new ProcessingJobVideo
            {
                Id = youtubeVideo.Id,
                Url = url,
                Title = youtubeVideo.Title,
                OriginalDescription = youtubeVideo.Description,
                Description = desc,
                Duration = youtubeVideo.Duration.GetValueOrDefault()
            }
        };

        return job.ParseChapters();
    }
}