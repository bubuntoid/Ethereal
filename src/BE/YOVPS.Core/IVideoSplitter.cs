using System.IO;
using System.Threading.Tasks;

namespace YOVPS.Core
{
    public interface IVideoSplitter
    {
        string TempPath { get; set; }
        string ExecutablesPath { get; set; }

        Task<ObjectWithName<byte[]>> DownloadZipAsync(string url, string description = null);

        Task<VideoChapter[]> GetChaptersByUrlAsync(string url);
        
        VideoChapter[] GetChaptersByDescription(string description);

        Task<ObjectWithName<Stream>> DownloadMp3Async(string url, string description, int index);
        
        Task<string> GetThumbnailUrlAsync(string url);
    }
}
