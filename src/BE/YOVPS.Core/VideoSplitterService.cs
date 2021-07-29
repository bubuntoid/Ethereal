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
using YOVPS.Core.Threading;

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
                throw description == null ? (Exception) new ChaptersNotFoundException() : new ChaptersParseException(); 
            
            logger.Info("Downloading video stream... \t");
            var videoFile = await SaveYouTubeVideoFileAsync(video);
            logger.Info("Done\n");

            var threadContextId = Guid.NewGuid();
            for (var i = 0; i < chapters.Count; i++)
            {
                var currentChapter = chapters.ElementAt(i);
                currentChapter.EndTimespan ??= video.Duration;

                var fileName = $"{currentChapter.Name}.{videoFile.Extension}";
                var outputPath = Path.Combine(videoFile.Directory, fileName);

                ThreadQueue.QueueTask(threadContextId,
                    FfmpegWrapper.SaveTrimmedAsync(videoFile.Path, outputPath, currentChapter, i, chapters.Count));
            }

            logger.Info("Waiting till TrimAndSaveToOutput threads will completed");
            await ThreadQueue.WhenAll(threadContextId, TimeSpan.FromMinutes(3));

            var zipMemoryStream = new MemoryStream();
            var zipArchive = new ZipArchive(zipMemoryStream, ZipArchiveMode.Create, false);
            for (var i = 0; i < chapters.Count; i++)
            {
                var currentChapter = chapters.ElementAt(i);

                var fileName = $"{currentChapter.Name}.{videoFile.Extension}";
                var outputPath = Path.Combine(videoFile.Directory, fileName);

                ComputationExtensions.ComputeElapsedTimeInMilliseconds(
                    $"CreateEntryFromFile | {currentChapter.Name} | {i + 1} / {chapters.Count}", () =>
                    {
                        logger.Info($"Creating zip entry for {currentChapter.Name}...");
                        zipArchive.CreateEntryFromFile(outputPath, fileName.Replace(videoFile.Extension, ".mp3"));
                    });
            }

            zipArchive.Dispose();
            zipMemoryStream.Close();

            Directory.Delete(videoFile.Directory, true);

            return new ObjectWithName<byte[]>(zipMemoryStream.ToArray(),
                $"{video.Title.RemoveIllegalCharacters()}.zip");
        }

        public async Task<VideoChapter[]> GetChaptersAsync(string url, string description = null, bool includeThumbnails = true)
        {
            var video = await client.Videos.GetAsync(url);
            var videoFile = includeThumbnails ? await SaveYouTubeVideoFileAsync(video, true) : null;
            
            var chapters = new VideoDescription(description ?? video.Description).ParseChapters();
            if (chapters == null ||chapters.Count == 0)
                throw description == null ? (Exception) new ChaptersNotFoundException() : new ChaptersParseException();

            var threadContextId = Guid.NewGuid();
            
            for (var i = 0; i < chapters.Count; i++)
            {
                var currentChapter = chapters.ElementAt(i);
                currentChapter.EndTimespan ??= video.Duration;

                if (includeThumbnails)
                {
                    var outputPath = Path.Combine(videoFile.Directory, $"{currentChapter.Name}.png");
                    ThreadQueue.QueueTask(threadContextId, FfmpegWrapper.SaveImageAsync(videoFile.Path, outputPath,
                        currentChapter, i,
                        chapters.Count));

                    // ThreadQueue.QueueTask(threadContextId, new Task(async () =>
                    // {
                    //     Console.WriteLine("started");
                    //     var outputPath = Path.Combine(videoFile.Directory, $"{currentChapter.Name}.png");
                    // 
                    //     await FfmpegWrapper.SaveImageAsync(videoFile.Path, outputPath, currentChapter, i,
                    //         chapters.Count);
                    //     
                    //     await using var stream = File.OpenRead(outputPath);
                    //     currentChapter.Thumbnail = stream.ReadFully();
                    // }));
                }
            }

            if (includeThumbnails)
            {
                await ThreadQueue.WhenAll(threadContextId, TimeSpan.FromMinutes(3));
                
                for (var i = 0; i < chapters.Count; i++)
                {
                    var currentChapter = chapters.ElementAt(i);

                    var outputPath = Path.Combine(videoFile.Directory, $"{currentChapter.Name}.png");

                    ComputationExtensions.ComputeElapsedTimeInMilliseconds(
                        $"Generating thumbnail | {currentChapter.Name} | {i + 1} / {chapters.Count}", async () =>
                        {
                            await using var stream = File.OpenRead(outputPath);
                            currentChapter.Thumbnail = stream.ReadFully();
                        });
                }
                
                Directory.Delete(videoFile.Directory, true);
            }

            return chapters.ToArray();
        }

        private async Task<YouTubeVideoFile> SaveYouTubeVideoFileAsync(IVideo video, bool lowestSize = false)
        {
            var manifest = await client.Videos.Streams.GetManifestAsync(video.Id);
            var streams = manifest.GetVideoStreams();
            var info = lowestSize
                ? streams.FirstOrDefault(x => x.Size.Bytes == streams.Min(s => s.Size.Bytes))
                : streams.FirstOrDefault(x => x.Container.Name == "mp4");
            var videoStream = await client.Videos.Streams.GetAsync(info);
            
            var directory = Path.Combine(tempPath, Guid.NewGuid().ToString());
            Directory.CreateDirectory(directory);            
            var path = Path.Combine(directory, $"__original__.{info.Container.Name}");
            var videoFileStream = File.Create(path);
            videoStream.Seek(0, SeekOrigin.Begin);
            await videoStream.CopyToAsync(videoFileStream);
            
            videoStream.Close();
            videoFileStream.Close();
            
            return new YouTubeVideoFile
            {
                Path = path,
                Extension = info.Container.Name,
                Directory = directory,
            };
        }

        public async Task<ObjectWithName<Stream>> DownloadMp3Async(string url, string description, int index)
        {
            logger.Info("Fetching video... \t");
            var video = await client.Videos.GetAsync(url);
            logger.Info("Done\n");

            var chapters = new VideoDescription(description ?? video.Description).ParseChapters();

            logger.Info("Downloading video stream... \t");
            var videoFile = await SaveYouTubeVideoFileAsync(video);
            logger.Info("Done\n");

            var currentChapter = chapters.ElementAt(index);
            currentChapter.EndTimespan ??= video.Duration;

            var fileName = $"{currentChapter.Name}.{videoFile.Extension}";
            var outputPath = Path.Combine(videoFile.Directory, fileName);

            await FfmpegWrapper.SaveTrimmedAsync(videoFile.Path, outputPath, currentChapter, index, chapters.Count);
            
            Directory.Delete(videoFile.Directory, true);
            return new ObjectWithName<Stream>(File.OpenRead(outputPath), outputPath);
        }
    }
}