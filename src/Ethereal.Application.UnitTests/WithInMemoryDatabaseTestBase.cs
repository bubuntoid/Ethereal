using System.IO;
using AutofacContrib.NSubstitute;
using AutoFixture;
using Ethereal.Application.YouTube;
using Ethereal.Domain;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Downloader;

namespace Ethereal.Application.UnitTests;

public class WithInMemoryDatabaseTestBase
{
    protected TestSettings Settings;

    protected WithInMemoryDatabaseTestBase()
    {
        // Setting up ffmpeg
        var ffmpegPath = Path.Combine(Directory.GetCurrentDirectory(), "ffmpeg");
        FFmpegDownloader.GetLatestVersion(FFmpegVersion.Full, ffmpegPath).GetAwaiter().GetResult();
        Settings = new TestSettings
        {
            FfmpegExecutablesPath = Path.Combine(ffmpegPath, "ffmpeg")
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
            .Provide<IYoutubeProvider>(new YtdlpProvider(Settings))
            .Build();

        ProcessingJobLogger.ProcessingJobLogger.CurrentSettings = Settings;
    }

    protected EtherealDbContext DbContext { get; }

    protected AutoSubstitute Substitute { get; }

    protected IFixture Fixture { get; }
}