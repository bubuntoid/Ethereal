using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Downloader;
using YOVPS.Common.Tests;

namespace YOVPS.Core.UnitTests
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
            var ffmpegPath = Path.Combine(Directory.GetCurrentDirectory(), "ffmpeg");
            await FFmpegDownloader.GetLatestVersion(FFmpegVersion.Full, ffmpegPath);
            FfmpegWrapper.ExecutablesPath = Path.Combine(ffmpegPath, "ffmpeg");
            FFmpeg.SetExecutablesPath(ffmpegPath);
            
            videoPath = ResourcesHelper.GetResourceFullPath("sample.mp4");
            tempOutputPath = Path.Combine(Directory.GetCurrentDirectory(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempOutputPath);
            
            chapter = new VideoChapter
            {
                StartTimespan = TimeSpan.FromMinutes(5),
                EndTimespan = TimeSpan.FromMinutes(10),
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
            var outputPath = Path.Combine(tempOutputPath, $"{Guid.NewGuid()}.mp3");
            await FfmpegWrapper.SaveTrimmedAsync(videoPath, outputPath, chapter, 0, 1);
            
            var mediaInfo = await FFmpeg.GetMediaInfo(outputPath);
            var videoDuration = mediaInfo.VideoStreams.First().Duration;

            Assert.That(videoDuration, Is.EqualTo(chapter.Duration));
        }
        
        [Test]
        public void SaveTrimmedAsync_VideoHasCorrectLength()
        {
            
        }
        
        [Test]
        public void SaveTrimmedAsync_VideoHasAudioTrack()
        {
            
        }
        
        [Test]
        public void SaveImageAsync_ImageSaved()
        {
            
        }
    }
}