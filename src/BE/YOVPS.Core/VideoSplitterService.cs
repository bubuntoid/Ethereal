using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using YoutubeExplode;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;
using YOVPS.Core.Exceptions;
using YOVPS.Core.Extensions;

namespace YOVPS.Core
{
    public class VideoSplitterService : IVideoSplitterService
    {
        private readonly Logger logger = LogManager.GetCurrentClassLogger();
        
        private readonly YoutubeClient client;
        private readonly string tempPath;
        private readonly string executablesPath;

        public VideoSplitterService(string tempPath, string executablesPath)
        {
            this.tempPath = tempPath;
            this.executablesPath = executablesPath;

            client = new YoutubeClient();

            FfmpegWrapper.ExecutablesPath = executablesPath;
        }

        public async Task<string> GetThumbnailUrlAsync(string url)
        {
            var video = await client.Videos.GetAsync(url);
            return $"https://img.youtube.com/vi/{video.Id}/maxresdefault.jpg";
        }

        public async Task<ObjectWithName<byte[]>> DownloadZipAsync(string url, string description = null)
        {
            logger.Info("Fetching video... \t");
            var video = await client.Videos.GetAsync(url);
            logger.Info("Done\n");

            var chapters = new VideoDescription(description ?? video.Description).ParseChapters();
            if (chapters.Count == 0)
                throw description == null ? new ChaptersNotFoundException() : new ChaptersParseException(); 
            
            logger.Info("Downloading video stream... \t");
            var videoStream = await GetYouTubeVideoStream(video);
            logger.Info("Done\n");

            var directory = Path.Combine(tempPath, Guid.NewGuid().ToString());
            Directory.CreateDirectory(directory);
            var path = Path.Combine(directory, $"__original__.{videoStream.Name}");
            var videoFileStream = File.Create(path);
            videoStream.Object.Seek(0, SeekOrigin.Begin);
            await videoStream.Object.CopyToAsync(videoFileStream);
            videoStream.Object.Close();
            videoFileStream.Close();

            var tasks = new List<Task>();
            for (var i = 0; i < chapters.Count; i++)
            {
                var currentChapter = chapters.ElementAt(i);
                currentChapter.EndTimespan ??= video.Duration;

                var fileName = $"{currentChapter.Name}.{videoStream.Name}";
                var outputPath = Path.Combine(directory, fileName);

                var task = FfmpegWrapper.TrimAndSaveToOutputAsync(path, outputPath, currentChapter, i, chapters.Count);
                tasks.Add(task);
            }

            logger.Info("Waiting till TrimAndSaveToOutput tasks will completed");
            await Task.WhenAll(tasks);

            var zipMemoryStream = new MemoryStream();
            var zipArchive = new ZipArchive(zipMemoryStream, ZipArchiveMode.Create, false);
            for (var i = 0; i < chapters.Count; i++)
            {
                var currentChapter = chapters.ElementAt(i);

                var fileName = $"{currentChapter.Name}.{videoStream.Name}";
                var outputPath = Path.Combine(directory, fileName);

                ComputationExtensions.ComputeElapsedTimeInMilliseconds(
                    $"CreateEntryFromFile | {currentChapter.Name} | {i + 1} / {chapters.Count}", () =>
                    {
                        logger.Info($"Creating zip entry for {currentChapter.Name}...");
                        zipArchive.CreateEntryFromFile(outputPath, fileName.Replace(videoStream.Name, ".mp3"));
                    });
            }

            zipArchive.Dispose();
            zipMemoryStream.Close();

            Directory.Delete(directory, true);

            return new ObjectWithName<byte[]>(zipMemoryStream.ToArray(),
                $"{video.Title.RemoveIllegalCharacters()}.zip");
        }

        public async Task<VideoChapter[]> GetChaptersAsync(string url, string description = null)
        {
            var video = await client.Videos.GetAsync(url);
            var chapters = new VideoDescription(description ?? video.Description).ParseChapters();
            for (var i = 0; i < chapters.Count; i++)
            {
                var currentChapter = chapters.ElementAt(i);
                currentChapter.EndTimespan ??= video.Duration;
            }

            return chapters.ToArray();
        }

        private async Task<ObjectWithName<Stream>> GetYouTubeVideoStream(IVideo video)
        {
            var manifest = await client.Videos.Streams.GetManifestAsync(video.Id);
            var info = manifest.GetAudioOnlyStreams().FirstOrDefault(x => x.Container.Name == "mp4");
            return new ObjectWithName<Stream>(await client.Videos.Streams.GetAsync(info), info.Container.Name);
        }

        public async Task<ObjectWithName<Stream>> DownloadMp3Async(string url, string description, int index)
        {
            logger.Info("Fetching video... \t");
            var video = await client.Videos.GetAsync(url);
            logger.Info("Done\n");

            var chapters = new VideoDescription(description ?? video.Description).ParseChapters();

            logger.Info("Downloading video stream... \t");
            var videoStream = await GetYouTubeVideoStream(video);
            logger.Info("Done\n");

            var directory = Path.Combine(tempPath, Guid.NewGuid().ToString());
            Directory.CreateDirectory(directory);
            var path = Path.Combine(directory, $"__original__.{videoStream.Name}");
            var videoFileStream = File.Create(path);
            videoStream.Object.Seek(0, SeekOrigin.Begin);
            await videoStream.Object.CopyToAsync(videoFileStream);
            videoStream.Object.Close();
            videoFileStream.Close();
            
            var currentChapter = chapters.ElementAt(index);
            currentChapter.EndTimespan ??= video.Duration;

            var fileName = $"{currentChapter.Name}.{videoStream.Name}";
            var outputPath = Path.Combine(directory, fileName);

            await FfmpegWrapper.TrimAndSaveToOutputAsync(path, outputPath, currentChapter, index, chapters.Count);
            
            return new ObjectWithName<Stream>(File.OpenRead(outputPath), outputPath);
        }
    }
}