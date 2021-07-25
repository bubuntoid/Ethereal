﻿using System;
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
            var videoStream = await GetYouTubeVideoStream(video);
            Console.Write("Done\n");

            var directory = Path.Combine(tempPath, Guid.NewGuid().ToString());
            Directory.CreateDirectory(directory);
            var path = Path.Combine(directory, $"__original__.{videoStream.Name}");
            var videoFileStream = File.Create(path);
            videoStream.Object.Seek(0, SeekOrigin.Begin);
            await videoStream.Object.CopyToAsync(videoFileStream);
            videoStream.Object.Close();
            videoFileStream.Close();

            for (var i = 0; i < chapters.Count; i++)
            {
                var currentChapter = chapters.ElementAt(i);
                currentChapter.EndTimespan ??= video.Duration;

                var fileName = $"{currentChapter.Name}.{videoStream.Name}";
                var outputPath = Path.Combine(directory, fileName);

                await FfmpegWrapper.TrimAndSaveToOutputAsync(path, outputPath, chapters, currentChapter, i);
            }

            Console.WriteLine("Waiting till TrimAndSaveToOutput tasks will completed");

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
                        Console.WriteLine($"Creating zip entry for {currentChapter.Name}...");
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
            Console.Write("Fetching video... \t");
            var video = await client.Videos.GetAsync(url);
            Console.Write("Done\n");

            var chapters = new VideoDescription(description ?? video.Description).ParseChapters();

            Console.Write("Downloading video stream... \t");
            var videoStream = await GetYouTubeVideoStream(video);
            Console.Write("Done\n");

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

            await FfmpegWrapper.TrimAndSaveToOutputAsync(path, outputPath, chapters, currentChapter, index);
            
            return new ObjectWithName<Stream>(File.OpenRead(outputPath), outputPath);
        }
    }
}