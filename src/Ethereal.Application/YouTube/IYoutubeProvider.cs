using System.Threading;
using System.Threading.Tasks;
using Ethereal.Domain.Entities;

namespace Ethereal.Application.YouTube
{
    public interface IYoutubeProvider 
    {
        public Task<YoutubeVideo> GetVideoAsync(string url);
        
        public Task DownloadAsync(ProcessingJob job, CancellationTokenSource cts);
    }
}