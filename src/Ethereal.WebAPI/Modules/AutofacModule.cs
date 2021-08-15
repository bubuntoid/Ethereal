using Autofac;
using AutoMapper;
using Ethereal.Application;
using Ethereal.Application.BackgroundJobs;
using Ethereal.Application.Commands;
using Ethereal.WebAPI.Filters;
using Ethereal.WebAPI.Settings;

namespace Ethereal.WebAPI.Modules
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            LoadCommands(builder);
            LoadBgJobs(builder);

            builder.RegisterType<ExceptionFilter>()
                .AsSelf();

            builder.RegisterType<SystemSettings>()
                .As<IEtherealSettings>();

            builder.Register(c =>
                    new MapperConfiguration(mc =>
                        {
                            mc.AddProfile(new MapperModule());
                        }).CreateMapper())
                .As<IMapper>()
                .SingleInstance();
        }

        private void LoadCommands(ContainerBuilder builder)
        {
            builder.RegisterType<SplitVideoCommand>()
                .AsSelf();
            
            builder.RegisterType<InitializeProcessingJobCommand>()
                .AsSelf();
            
            builder.RegisterType<FetchThumbnailsCommand>()
                .AsSelf();
            
            builder.RegisterType<FetchYoutubeVideoCommand>()
                .AsSelf();
            
            builder.RegisterType<ArchiveFilesCommand>()
                .AsSelf();
        }

        private void LoadBgJobs(ContainerBuilder builder)
        {
            builder.RegisterType<InitializeJob>()
                .AsSelf();
        }
        
    }
}