using System.IO;
using System.Threading.Tasks;

namespace Ethereal.Application
{
    public interface IVideoSplitterService
    {
        Task<ObjectWithName<byte[]>> DownloadZipAsync(string url, string description = null);

        Task<VideoChapter[]> GetChaptersAsync(string url, string description = null, bool includeThumbnails = true);

        Task<ObjectWithName<Stream>> DownloadMp3Async(string url, string description, int index);
        
        Task<string> GetThumbnailUrlAsync(string url);
    }
}
