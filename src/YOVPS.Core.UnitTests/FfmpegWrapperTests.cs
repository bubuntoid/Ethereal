using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using Xabe.FFmpeg.Downloader;
using YOVPS.Common.Tests;

namespace YOVPS.Core.UnitTests
{
    [TestFixture]
    public class FfmpegWrapperTests
    {
        private string videoPath; 
        
        [SetUp]
        public async Task SetUp()
        {
            var ffmpegPath = Path.Combine(Directory.GetCurrentDirectory(), "ffmpeg");
            await FFmpegDownloader.GetLatestVersion(FFmpegVersion.Full, ffmpegPath);

            FfmpegWrapper.ExecutablesPath = ffmpegPath;
            videoPath = ResourcesHelper.GetResourceFullPath("sample.mp4");
        }

        [Test]
        public void SaveTrimmedAsync_VideoSaved()
        {
            
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