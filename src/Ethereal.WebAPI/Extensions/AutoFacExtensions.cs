using Autofac;
using Ethereal.Application.YouTube;
using Ethereal.WebAPI.Settings;

#pragma warning disable 618

namespace Ethereal.WebAPI.Extensions;

public static class AutoFacExtensions
{
    public static void ResolveYoutubeProvider(this ContainerBuilder builder, SystemSettings settings)
    {
        switch (settings.YouTubeProvider)
        {
            case "YouTubeExplode":
                builder.RegisterType<YoutubeExplodeProvider>()
                    .As<IYoutubeProvider>();
                break;

            case "YouTubeExtractor":
                builder.RegisterType<YoutubeExtractorProvider>()
                    .As<IYoutubeProvider>();
                break;

            case "SharpGrabber":
                builder.RegisterType<SharpGrabberProvider>()
                    .As<IYoutubeProvider>();
                break;

            case "yt-dlp":
                builder.RegisterType<YtdlpProvider>()
                    .As<IYoutubeProvider>();
                break;

            default:
                builder.RegisterType<YoutubeExplodeProvider>()
                    .As<IYoutubeProvider>();
                break;
        }
    }
}