﻿using System;
using System.Reflection;
using AutofacContrib.NSubstitute;
using AutoFixture;
using Ethereal.Domain.Migrations;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors;
using NSubstitute;

namespace Ethereal.Domain.UnitTests
{
    [TestFixture]
    public abstract class WithRealDatabaseTestBase
    {
        private readonly TestDatabaseSettings settings;

        protected EtherealDbContext DbContext { get; }
        protected IFixture Fixture { get; }
        protected AutoSubstitute Substitute { get; }
        
        protected WithRealDatabaseTestBase()
        {
            settings = new TestDatabaseSettings();
            DbContext = new EtherealDbContext(settings);
            
            Fixture = new Fixture();
            Fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            Fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            Substitute = AutoSubstitute
                .Configure()
                .Provide(DbContext)
                .Build();
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            // Instantiate the runner
            var runner = CreateServices().GetRequiredService<IMigrationRunner>();

            // Execute the migrations
            runner.MigrateUp();
        }
        
        /// <summary>
        /// Configure the dependency injection services
        /// </summary>
        private IServiceProvider CreateServices()
        {
            return new ServiceCollection()
                // Add common FluentMigrator services
                .AddLogging(c => c.AddFluentMigratorConsole())
                .AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    .AddPostgres()
                    // Set the connection string
                    .WithGlobalConnectionString(settings.ConnectionString)
                    // Define the assembly containing the migrations
                    .ScanIn(typeof(InitializeDatabaseMigration).Assembly).For.Migrations())
                // Enable logging to console in the FluentMigrator way
                .AddLogging(lb => lb.AddFluentMigratorConsole())
                // Build the service provider
                .BuildServiceProvider(false);
        }
    }
}