﻿using System;
using System.IO;
using System.Threading.Tasks;
using Ethereal.Application.Commands;
using Ethereal.Application.Exceptions;
using Ethereal.Application.Extensions;
using Ethereal.Application.UnitTests.Builders;
using Ethereal.Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Ethereal.Application.UnitTests.Tests.Commands
{
    [TestFixture]
    public class SplitVideoCommandTests : WithInMemoryDatabaseTestBase
    {
        private SplitVideoCommand command;
        private ProcessingJob job;
        
        [SetUp]
        public async Task SetUp()
        {
            command = Substitute.Resolve<SplitVideoCommand>();
            job = new ProcessingJobBuilder(Fixture, Settings).Build();
            await DbContext.ProcessingJobs.AddAsync(job);
            await DbContext.SaveChangesAsync();
            
            await Substitute.Resolve<FetchYoutubeVideoCommand>().ExecuteAsync(job.Id);
            await Substitute.Resolve<FetchThumbnailsCommand>().ExecuteAsync(job.Id);
        }

        [Test]
        public void JobDoesNotExists_ErrorExpected()
        {
            var ex = Assert.CatchAsync<NotFoundException>(() => command.ExecuteAsync(Guid.NewGuid()));
        }

        [Test]
        public async Task JobExists_StatusChanged()
        {
            await command.ExecuteAsync(job.Id);

            var updatedJob = await DbContext.ProcessingJobs.FirstOrDefaultAsync(j => j.Id == job.Id);
            
            updatedJob.Status.Should().BeEquivalentTo(ProcessingJobStatus.Processing);
            updatedJob.CurrentStepDescription.Should().BeEquivalentTo("Splitting succeeded");
        }
        
        [Test]
        // ReSharper disable once IdentifierTypo
        public async Task VideoSplitted_FilesExistsLocally()
        {
            var chapters = job.ParseChapters();

            await command.ExecuteAsync(job.Id);

            foreach (var chapter in chapters)
            {
                var path = Path.Combine(job.GetChapterLocalFilePath(chapter));
                Assert.That(File.Exists(path));
            }
        }
    }
}