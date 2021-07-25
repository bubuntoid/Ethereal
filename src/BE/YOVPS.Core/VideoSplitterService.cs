using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
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
            Console.Write("Fetching video... \t");
            var video = await client.Videos.GetAsync(url);
            Console.Write("Done\n");

            var chapters = new VideoDescription(description ?? video.Description).ParseChapters();

            Console.Write("Downloading video stream... \t");
            var stream = await GetYouTubeVideoStream(video);
            Console.Write("Done\n");

            var directory = Path.Combine(tempPath, Guid.NewGuid().ToString());
            Directory.CreateDirectory(directory);
            var path = Path.Combine(directory, "__original__.mp3");
            var videoFileStream = File.Create(path);
            stream.Seek(0, SeekOrigin.Begin);
            await stream.CopyToAsync(videoFileStream);
            stream.Close();
            videoFileStream.Close();

            var tasks = new List<Task>();
            for (var i = 0; i < chapters.Count; i++)
            {
                var currentChapter = chapters.ElementAt(i);
                currentChapter.EndTimespan ??= video.Duration;

                var fileName = $"{currentChapter.Name}.mp3";
                var outputPath = Path.Combine(directory, fileName);

                var task = FfmpegWrapper.TrimAndSaveToOutputAsync(path, outputPath, chapters, currentChapter, i);
                tasks.Add(task);
            }

            Console.WriteLine("Waiting till TrimAndSaveToOutput tasks will completed");
            await Task.WhenAll(tasks);
            tasks.Clear();

            var zipMemoryStream = new MemoryStream();
            var zipArchive = new ZipArchive(zipMemoryStream, ZipArchiveMode.Create, false);
            for (var i = 0; i < chapters.Count; i++)
            {
                var currentChapter = chapters.ElementAt(i);
                currentChapter.EndTimespan ??= video.Duration;

                var fileName = $"{currentChapter.Name}.mp3";
                var outputPath = Path.Combine(directory, fileName);

                ComputationExtensions.ComputeElapsedTimeInMilliseconds(
                    $"CreateEntryFromFile | {currentChapter.Name} | {i + 1} / {chapters.Count}", () =>
                    {
                        Console.WriteLine($"Creating zip entry for {currentChapter.Name}...");
                        zipArchive.CreateEntryFromFile(outputPath, fileName);
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

        private async Task<Stream> GetYouTubeVideoStream(IVideo video)
        {
            var manifest = await client.Videos.Streams.GetManifestAsync(video.Id);
            var info = manifest.GetAudioOnlyStreams().GetWithHighestBitrate();
            return await client.Videos.Streams.GetAsync(info);
        }

        public async Task<ObjectWithName<Stream>> DownloadMp3Async(string url, string description, int index)
        {
            return null;

            // var video = await client.Videos.GetAsync(url);
            // var chapters = new VideoDescription(string.IsNullOrEmpty(description) ? video.Description : description)
            //     .ParseChapters();
            // var stream = await GetYouTubeVideoStream(video);
            // 
            // var directory = Path.Combine(tempPath, Guid.NewGuid().ToString());
            // Directory.CreateDirectory(directory);
            // var path = Path.Combine(directory, "__original__");
            // var videoFileStream = File.Create(path);
            // stream.Seek(0, SeekOrigin.Begin);
            // await stream.CopyToAsync(videoFileStream);
            // stream.Close();
            // videoFileStream.Close();
            // 
            // var currentChapter = chapters.ElementAt(index);
            // var startTimespan = currentChapter.StartTimespan;
            // var endTimespan = index == chapters.Count - 1
            //     // ReSharper disable once PossibleInvalidOperationException
            //     ? video.Duration.Value
            //     : chapters.ElementAt(index + 1).StartTimespan;
            // var duration = endTimespan - startTimespan;
            // 
            // var filename = $"{currentChapter.Name}.mp3";
            // var outputPath = Path.Combine(directory, filename);
            // 
            // FfmpegWrapper.TrimAndSaveToOutput(path, outputPath, startTimespan, duration);
            // 
            // return new ObjectWithName<Stream>(File.OpenRead(outputPath), filename);
        }
    }
}