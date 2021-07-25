using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Xabe.FFmpeg;
using YoutubeExplode;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;
using YOVPS.Core.Extensions;

namespace YOVPS.Core
{
    public class VideoSplitterService : IVideoSplitterService
    {
        private readonly YoutubeClient client;
        private readonly string tempPath;
        private readonly string executablesPath;
        
        public VideoSplitterService(string tempPath, string executablesPath)
        {
            this.tempPath = tempPath;
            this.executablesPath = executablesPath;
            FFmpeg.SetExecutablesPath(executablesPath);
            
            client = new YoutubeClient();
        }

        public async Task<string> GetThumbnailUrlAsync(string url)
        {
            var video = await client.Videos.GetAsync(url);
            return $"https://img.youtube.com/vi/{video.Id}/maxresdefault.jpg";
        }
        
        public async Task<ObjectWithName<byte[]>> DownloadZipAsync(string url, string description = null)
        {
            var video = await client.Videos.GetAsync(url);
            var chapters = new VideoDescription(description ?? video.Description).ParseChapters();
            var stream = await GetYouTubeVideoStream(video);

            var directory = Path.Combine(tempPath, Guid.NewGuid().ToString());
            Directory.CreateDirectory(directory);
            var path = Path.Combine(directory, "__original__");
            var videoFileStream = File.Create(path);
            stream.Seek(0, SeekOrigin.Begin);
            await stream.CopyToAsync(videoFileStream);
            stream.Close();
            videoFileStream.Close();

            var zipMemoryStream = new MemoryStream();
            var zipArchive = new ZipArchive(zipMemoryStream, ZipArchiveMode.Create, false);
            for (var i = 0; i < chapters.Count; i++)
            {
                var currentChapter = chapters.ElementAt(i);
                var startTimespan = currentChapter.ParsedTimespan;
                var endTimespan = i == chapters.Count - 1
                    // ReSharper disable once PossibleInvalidOperationException
                    ? video.Duration.Value
                    : chapters.ElementAt(i + 1).ParsedTimespan;
                var duration = endTimespan - startTimespan;

                var fileName = $"{currentChapter.ParsedName}.mp3";
                var outputPath = Path.Combine(directory, fileName);

                var mediaInfo = await FFmpeg.GetMediaInfo(path);
                var mediaInfoStream = mediaInfo.AudioStreams.First().Split(startTimespan, duration);
                await FFmpeg.Conversions.New()
                    .AddStream(mediaInfoStream)
                    .SetOutput(outputPath)
                    .Start();

                zipArchive.CreateEntryFromFile(outputPath, fileName);
            }

            zipArchive.Dispose();
            zipMemoryStream.Close();

            Directory.Delete(directory, true);
            return new ObjectWithName<byte[]>(zipMemoryStream.ToArray(),
                $"{video.Title.RemoveIllegalCharacters()}.zip");
        }

        public async Task<VideoChapter[]> GetChaptersByUrlAsync(string url)
        {
            var video = await client.Videos.GetAsync(url);
            return new VideoDescription(video.Description).ParseChapters().ToArray();
        }

        public VideoChapter[] GetChaptersByDescription(string description)
        {
            return new VideoDescription(description).ParseChapters().ToArray();
        }

        private async Task<Stream> GetYouTubeVideoStream(IVideo video)
        {
            var manifest = await client.Videos.Streams.GetManifestAsync(video.Id);
            var info = manifest.GetAudioOnlyStreams().GetWithHighestBitrate();
            return await client.Videos.Streams.GetAsync(info);            
        }

        public async Task<ObjectWithName<Stream>> DownloadMp3Async(string url, string description, int index)
        {
            var video = await client.Videos.GetAsync(url);
            var chapters = new VideoDescription(string.IsNullOrEmpty(description) ? video.Description : description)
                .ParseChapters();
            var stream = await GetYouTubeVideoStream(video);
            
            var directory = Path.Combine(tempPath, Guid.NewGuid().ToString());
            Directory.CreateDirectory(directory);
            var path = Path.Combine(directory, "__original__");
            var videoFileStream = File.Create(path);
            stream.Seek(0, SeekOrigin.Begin);
            await stream.CopyToAsync(videoFileStream);
            stream.Close();
            videoFileStream.Close();
            
            var currentChapter = chapters.ElementAt(index);
            var startTimespan = currentChapter.ParsedTimespan;
            var endTimespan = index == chapters.Count - 1
                // ReSharper disable once PossibleInvalidOperationException
                ? video.Duration.Value
                : chapters.ElementAt(index + 1).ParsedTimespan;
            var duration = endTimespan - startTimespan;
            
            var filename = $"{currentChapter.ParsedName}.mp3";
            var outputPath = Path.Combine(directory, filename);
            
            var mediaInfo = await FFmpeg.GetMediaInfo(path);
            var mediaInfoStream = mediaInfo.AudioStreams.First().Split(startTimespan, duration);
            await FFmpeg.Conversions.New()
                .AddStream(mediaInfoStream)
                .SetOutput(outputPath)
                .Start();

            return new ObjectWithName<Stream>(File.OpenRead(outputPath), filename);
        }
    }
}
