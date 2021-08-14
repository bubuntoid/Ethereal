using Autofac;
using AutoMapper;
using Ethereal.Application;
using Ethereal.WebAPI.Filters;
using Ethereal.WebAPI.Settings;

namespace Ethereal.WebAPI.Modules
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

            builder.RegisterType<SystemSettings>()
                .AsSelf();

            builder.Register<VideoSplitterService>(c =>
                {
                    var settings = c.Resolve<SystemSettings>();
                    return new VideoSplitterService(settings.TempPath, settings.ExecutablesPath);
                })
                .As<IVideoSplitterService>();

            builder.Register(c =>
                    new MapperConfiguration(mc =>
                        {
                            mc.AddProfile(new MapperModule());
                        }).CreateMapper())
                .As<IMapper>()
                .SingleInstance();
        }
    }
}