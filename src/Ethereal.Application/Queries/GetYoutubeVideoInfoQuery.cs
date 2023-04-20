using System.Threading.Tasks;
using Ethereal.Application.YouTube;
using Ethereal.Domain.Entities;

namespace Ethereal.Application.Queries;

public class GetYoutubeVideoInfoQuery
{
    private readonly IYoutubeProvider youtubeProvider;

    public GetYoutubeVideoInfoQuery(IYoutubeProvider youtubeProvider)
    {
        this.youtubeProvider = youtubeProvider;
    }

    public async Task<ProcessingJobVideo> ExecuteAsync(string url)
    {
        var youtubeVideo = await youtubeProvider.GetVideoAsync(url);
        return new ProcessingJobVideo
        {
            Description = youtubeVideo.Description,
            Title = youtubeVideo.Title
        };
    }
}