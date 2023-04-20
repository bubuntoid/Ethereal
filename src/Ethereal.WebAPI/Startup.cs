using System;
using System.Linq;
using Autofac;
using Ethereal.Application.BackgroundJobs;
using Ethereal.Application.ProcessingJobLogger;
using Ethereal.Domain.Entities;
using Ethereal.Domain.Migrations;
using Ethereal.WebAPI.Contracts;
using Ethereal.WebAPI.Extensions;
using Ethereal.WebAPI.Filters;
using Ethereal.WebAPI.Settings;
using Ethereal.WebAPI.SignalR;
using FluentMigrator.Runner;
using FluentValidation.AspNetCore;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;

namespace Ethereal.WebAPI;

public class Startup
{
    private const string SwaggerVersion = "v2.1.0.0";

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public ILifetimeScope AutofacContainer { get; set; }
    private IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddFluentValidation(fv =>
            fv.RegisterValidatorsFromAssemblyContaining<InitializeJobRequestDto>());

        services.AddControllers()
            .AddNewtonsoftJson(opts =>
                opts.SerializerSettings.Converters.Add(new StringEnumConverter()));

        services.AddSwaggerGen(c => { c.SwaggerDoc(SwaggerVersion, new OpenApiInfo { Title = "Ethereal WebAPI", Version = SwaggerVersion }); });

        services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigin",
                builder =>
                {
                    var settings = new CorsSettings(Configuration);
                    builder.WithOrigins(settings.Origins)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
        });

        var databaseSettings = new DatabaseSettings(Configuration);
        ProcessingJobLogger.CurrentSettings = new SystemSettings(Configuration);
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

        services.AddHangfireServer(s => { s.WorkerCount = 20; });

        services.AddSwaggerGenNewtonsoftSupport();

        services.AddSignalR();
    }

    public void ConfigureContainer(ContainerBuilder builder)
    {
        // Register your own things directly with Autofac here. Don't
        // call builder.Populate(), that happens in AutofacServiceProviderFactory
        // for you.
        builder.RegisterModule(new AutofacModule());
        builder.ResolveYoutubeProvider(new SystemSettings(Configuration));
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
            app.UseSwaggerUI(c => c.SwaggerEndpoint($"/swagger/{SwaggerVersion}/swagger.json", $"WebAPI {SwaggerVersion}"));
        }

        app.UseRouting();

        app.UseAuthorization();

        app.UseHangfireDashboard("/hangfire", new DashboardOptions
        {
            Authorization = new[] { new NoDashboardAuthorizationFilter() }
        });

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHub<ProcessingJobLoggerHub>("/logger");
            endpoints.MapFallbackToAreaController("Index", "Index", "");
        });

        runner?.MigrateUp();

        RecurringJob.AddOrUpdate<GarbageCleanerJob>("garbageCleaner",
            bgJob => bgJob.Execute(TimeSpan.FromMinutes(10)), Cron.Hourly);

        async void OnLog(ProcessingJob job, string log)
        {
            var connectionIds = SignalR.SignalR.Sessions.Where(s => s.ProcessingJobId == job.Id).ToList();
            await hubContext.Clients.Clients(connectionIds.Select(s => s.ConnectionId))
                .SendAsync("onReceiveLog", new { log, status = $"{job.Status}" });
        }

        ProcessingJobLogger.OnLog = OnLog;
    }
}