using System.IO;
using AutofacContrib.NSubstitute;
using AutoFixture;
using Ethereal.Domain;
using NUnit.Framework;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Downloader;
using YoutubeExplode;

namespace Ethereal.Application.UnitTests
{
    [Ignore("CI/CD")]
    [TestFixture]
    public class WithInMemoryDatabaseTestBase
    {
        protected EtherealDbContext DbContext { get; } 
        
        protected AutoSubstitute Substitute { get; }

        protected IFixture Fixture { get; }

        protected TestSettings Settings;
        
        protected WithInMemoryDatabaseTestBase()
        {
            // Setting up ffmpeg
            var ffmpegPath = Path.Combine(Directory.GetCurrentDirectory(), "ffmpeg");
            FFmpegDownloader.GetLatestVersion(FFmpegVersion.Full, ffmpegPath).GetAwaiter().GetResult();
            Settings = new TestSettings
            {
                ExecutablesPath = Path.Combine(ffmpegPath, "ffmpeg")
            };
            FFmpeg.SetExecutablesPath(ffmpegPath);
            
            DbContext = new EtherealInMemoryDatabase();
            
            Fixture = new Fixture();
            Fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            Fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            
            Substitute = AutoSubstitute
                .Configure()
                .Provide(DbContext)
                .Provide(new FfmpegWrapper(Settings))
                .Provide(new YoutubeClient())
                .Build();

            ProcessingJobLogger.ProcessingJobLogger.CurrentSettings = Settings;
        }
    }
}