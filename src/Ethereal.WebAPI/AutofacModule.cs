using System.ComponentModel;
using Autofac;
using AutoMapper;
using Ethereal.Application;
using Ethereal.Application.BackgroundJobs;
using Ethereal.Application.Commands;
using Ethereal.Application.Queries;
using Ethereal.Domain;
using Ethereal.WebAPI.Filters;
using Ethereal.WebAPI.Settings;
using Hangfire;
using Microsoft.AspNetCore.Components.RenderTree;
using YoutubeExplode;

namespace Ethereal.WebAPI
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            LoadQueries(builder);
            LoadCommands(builder);
            LoadBgJobs(builder);
            LoadSettings(builder);

            builder.RegisterType<EtherealDbContext>()
                .AsSelf()
                .InstancePerLifetimeScope();
            
            builder.RegisterType<ExceptionFilter>()
                .AsSelf();

            builder.RegisterType<FfmpegWrapper>()
                .AsSelf();
            
            builder.Register(c =>
                    new MapperConfiguration(mc =>
                        {
                            mc.AddProfile(new MapperProfile());
                        }).CreateMapper())
                .As<IMapper>()
                .SingleInstance();

            builder.Register(c => new YoutubeClient())
                .AsSelf();
        }

        private void LoadQueries(ContainerBuilder builder)
        {
            builder.RegisterType<GetThumbnailFilePathQuery>()
                .AsSelf();
            
            builder.RegisterType<GetZipArchiveFilePathQuery>()
                .AsSelf();
            
            builder.RegisterType<GetAudioFilePathQuery>()
                .AsSelf();
            
            builder.RegisterType<GetProcessingJobQuery>()
                .AsSelf();

            builder.RegisterType<GetLogFilePathQuery>()
                .AsSelf();
        }
        
        private void LoadCommands(ContainerBuilder builder)
        {
            builder.RegisterType<ConvertVideoCommand>()
                .AsSelf()
                .InstancePerLifetimeScope();
            
            builder.RegisterType<InitializeProcessingJobCommand>()
                .AsSelf()
                .InstancePerLifetimeScope();
            
            builder.RegisterType<FetchThumbnailsCommand>()
                .AsSelf()
                .InstancePerLifetimeScope();
            
            builder.RegisterType<FetchYoutubeVideoCommand>()
                .AsSelf()
                .InstancePerLifetimeScope();
            
            builder.RegisterType<ArchiveFilesCommand>()
                .AsSelf()
                .InstancePerLifetimeScope();
        }

        private void LoadBgJobs(ContainerBuilder builder)
        {
            builder.RegisterType<InitializeJob>()
                .AsSelf()
                .InstancePerLifetimeScope();

            builder.RegisterType<DestructJob>()
                .AsSelf()
                .InstancePerLifetimeScope();
        }

        private void LoadSettings(ContainerBuilder builder)
        {
            builder.RegisterType<SystemSettings>()
                .As<IEtherealSettings>();

            builder.RegisterType<DatabaseSettings>()
                .As<IDatabaseSettings>();
        }
    }
}