using System;
using System.IO;
using System.Threading.Tasks;
using AutoFixture;
using Ethereal.Application.Commands;
using Ethereal.Application.Exceptions;
using Ethereal.Application.Extensions;
using Ethereal.Application.UnitTests.Builders;
using Ethereal.Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using NUnit.Framework;

namespace Ethereal.Application.UnitTests.Tests.Commands
{
    [TestFixture]
    public class FetchThumbnailsCommandTests : WithInMemoryDatabaseTestBase
    {
        private FetchThumbnailsCommand command;
        private ProcessingJob job;
        
        [SetUp]
        public async Task SetUp()
        {
            command = Substitute.Resolve<FetchThumbnailsCommand>();
            job = new ProcessingJobBuilder(Fixture, Settings).Build();
            await DbContext.ProcessingJobs.AddAsync(job);
            await DbContext.SaveChangesAsync();

            await Substitute.Resolve<FetchYoutubeVideoCommand>().ExecuteAsync(job.Id);
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
            
            updatedJob.Status.Should().BeEquivalentTo(ProcessingJobStatus.FetchingThumbnail);
            updatedJob.CurrentStepDescription.Should().BeEquivalentTo("Thumbnails fetched");
        }
        
        [Test]
        public async Task JobExists_ThumbnailsExistsLocally()
        {
            var chapters = job.ParseChapters();
            
            await command.ExecuteAsync(job.Id);

            foreach (var chapter in chapters)
            {
                var path = Path.Combine(job.GetLocalThumbnailsDirectoryPath(), chapter.Index + ".png");
                Assert.That(File.Exists(path));
            }
        }
    }
}