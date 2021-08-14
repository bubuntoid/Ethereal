using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Ethereal.Application.Extensions;
using NUnit.Framework;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Downloader;
using YOVPS.Common.Tests;

namespace Ethereal.Application.UnitTests
{
    [TestFixture]
    public class FfmpegWrapperTests
    {
        private string videoPath;
        private string tempOutputPath;
        private VideoChapter chapter;
        
        [SetUp]
        public async Task SetUp()
        {
            // Setting up ffmpeg
            var ffmpegPath = Path.Combine(Directory.GetCurrentDirectory(), "ffmpeg");
            await FFmpegDownloader.GetLatestVersion(FFmpegVersion.Full, ffmpegPath);
            FfmpegWrapper.ExecutablesPath = Path.Combine(ffmpegPath, "ffmpeg");
            FFmpeg.SetExecutablesPath(ffmpegPath);
            
            // Creating temp directory to work with
            tempOutputPath = Path.Combine(Directory.GetCurrentDirectory(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempOutputPath);
            
            // Copying video to temp folder
            videoPath = Path.Combine(tempOutputPath, "sample.mp4");
            await ResourcesHelper.SaveResourceToFileAsync("sample.mp4", videoPath);
            
            // Sample chapter
            chapter = new VideoChapter
            {
                StartTimespan = TimeSpan.FromMinutes(1),
                EndTimespan = TimeSpan.FromMinutes(2),
            };
        }

        [TearDown]
        public void TearDown()
        {
            Directory.Delete(tempOutputPath, true);
        }
        
        [Test]
        public async Task SaveTrimmedAsync_VideoSaved()
        {
            var filename = $"{Guid.NewGuid()}.mp4";
            var outputPath = Path.Combine(tempOutputPath, filename);
            await FfmpegWrapper.SaveTrimmedAsync(videoPath, outputPath, chapter, 0, 1);
            
            var mediaInfo = await FFmpeg.GetMediaInfo(outputPath);
            var videoDuration = mediaInfo.AudioStreams.First().Duration;

            var files = Directory.GetFiles(tempOutputPath);
            Assert.That(files.Any(file => file.EndsWith(filename)));
        }
        
        [Test]
        public async Task SaveTrimmedAsync_VideoHasCorrectLength()
        {
            var outputPath = Path.Combine(tempOutputPath, $"{Guid.NewGuid()}.mp4");
            await FfmpegWrapper.SaveTrimmedAsync(videoPath, outputPath, chapter, 0, 1);
            
            var mediaInfo = await FFmpeg.GetMediaInfo(outputPath);
            var videoDuration = mediaInfo.AudioStreams.First().Duration;

            // ReSharper disable once PossibleInvalidOperationException
            Assert.That(videoDuration.ClearMilliseconds(), Is.EqualTo(chapter.Duration.Value.ClearMilliseconds()));
        }
        
        [Test]
        public async Task SaveTrimmedAsync_TrimmedVideoHasNoVideoStreamsLeft()
        {
            var outputPath = Path.Combine(tempOutputPath, $"{Guid.NewGuid()}.mp4");
            await FfmpegWrapper.SaveTrimmedAsync(videoPath, outputPath, chapter, 0, 1);
            
            var mediaInfo = await FFmpeg.GetMediaInfo(outputPath);
            Assert.That(mediaInfo.VideoStreams.Count(), Is.EqualTo(0));
        }
        
        [Test]
        public async Task SaveImageAsync_ImageSaved()
        {
            var filename = $"{Guid.NewGuid()}.jpeg";
            var outputPath = Path.Combine(tempOutputPath, filename);
            await FfmpegWrapper.SaveImageAsync(videoPath, outputPath, chapter, 0, 1);
            
            var files = Directory.GetFiles(tempOutputPath);
            Assert.That(files.Any(file => file.EndsWith(filename)));
        }
    }
}