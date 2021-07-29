using Autofac;
using Microsoft.Extensions.Logging;
using YOVPS.Core;
using YOVPS.WebAPI.Filters;
using YOVPS.WebAPI.Settings;

namespace YOVPS.WebAPI.Modules
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // The generic ILogger<TCategoryName> service was added to the ServiceCollection by ASP.NET Core.
            // It was then registered with Autofac using the Populate method. All of this starts
            // with the services.AddAutofac() that happens in Program and registers Autofac
            // as the service provider.

            builder.RegisterType<ExceptionFilter>()
                .AsSelf();
            
            builder.RegisterType<FfmpegSettings>()
                .AsSelf();
            
            builder.Register<VideoSplitterService>(c =>
                {
                    var settings = c.Resolve<FfmpegSettings>();
                    return new VideoSplitterService(settings.TempPath, settings.ExecutablesPath);
                })
                .As<IVideoSplitterService>();
        }
    }
}