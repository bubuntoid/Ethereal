using System.IO;
using System.Threading.Tasks;

namespace YOVPS.Core
{
    public interface IVideoSplitterService
    {
        Task<ObjectWithName<byte[]>> DownloadZipAsync(string url, string description = null);

        Task<VideoChapter[]> GetChaptersAsync(string url, string description = null);

        Task<ObjectWithName<Stream>> DownloadMp3Async(string url, string description, int index);
        
        Task<string> GetThumbnailUrlAsync(string url);
    }
}
