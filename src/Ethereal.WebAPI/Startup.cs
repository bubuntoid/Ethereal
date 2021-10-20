using System;
using System.Linq;
using Autofac;
using Ethereal.Application.BackgroundJobs;
using Ethereal.Application.ProcessingJobLogger;
using Ethereal.Domain.Migrations;
using Ethereal.WebAPI.Contracts;
using Ethereal.WebAPI.Settings;
using Ethereal.WebAPI.SignalR;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using FluentMigrator.Runner;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json.Converters;

namespace Ethereal.WebAPI
{
    public class Startup
    {
        public ILifetimeScope AutofacContainer { get; set; }
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddFluentValidation(fv =>
                fv.RegisterValidatorsFromAssemblyContaining<InitializeJobRequestDto>());

            services.AddControllers()
                .AddNewtonsoftJson(opts =>
                    opts.SerializerSettings.Converters.Add(new StringEnumConverter()));

            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo {Title = "WebAPI", Version = "v1"}); });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder =>
                    {
                        var settings = new CorsSettings(this.Configuration);
                        builder.WithOrigins(settings.Origins)
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials();
                    });
            });

            var databaseSettings = new DatabaseSettings(this.Configuration);
            ProcessingJobLogger.CurrentSettings = new SystemSettings(this.Configuration);
            ProcessingJobLogger.DatabaseSettings = databaseSettings;
            
            services
                .AddLogging(c => c.AddFluentMigratorConsole())
                .AddFluentMigratorCore()
                .ConfigureRunner(c => c
                    .AddPostgres()
                    .WithGlobalConnectionString(databaseSettings.ConnectionString)
                    .ScanIn(typeof(InitializeDatabaseMigration).Assembly).For.All());

            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UsePostgreSqlStorage(databaseSettings.ConnectionString, new PostgreSqlStorageOptions()));
            services.AddHangfireServer();

            services.AddSwaggerGenNewtonsoftSupport();

            services.AddSignalR();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            // Register your own things directly with Autofac here. Don't
            // call builder.Populate(), that happens in AutofacServiceProviderFactory
            // for you.
            builder.RegisterModule(new AutofacModule());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IMigrationRunner runner,
            IHubContext<ProcessingJobLoggerHub> hubContext)
        {
            app.UseCors("AllowSpecificOrigin");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebAPI v1"));
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseHangfireServer(new BackgroundJobServerOptions
            {
                WorkerCount = 10,
            });
            app.UseHangfireDashboard();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ProcessingJobLoggerHub>("/logger");
            });

            runner?.MigrateUp();

            RecurringJob.AddOrUpdate<GarbageCleanerJob>("garbageCleaner",
                bgJob => bgJob.Execute(TimeSpan.FromMinutes(10)), Cron.Hourly);

            ProcessingJobLogger.OnLog = async (job, log) =>
            {
                var connectionIds = SignalR.SignalR.Sessions.Where(s => s.ProcessingJobId == job.Id).ToList();
                await hubContext.Clients.Clients(connectionIds.Select(s => s.ConnectionId))
                    .SendAsync("onReceiveLog", new {log, status = $"{job.Status}"});
            };
        }
    }
}