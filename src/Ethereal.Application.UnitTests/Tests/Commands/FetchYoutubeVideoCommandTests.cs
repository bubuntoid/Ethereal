using System;
using System.IO;
using System.Threading.Tasks;
using AutoFixture;
using Ethereal.Application.Commands;
using Ethereal.Application.UnitTests.Builders;
using Ethereal.Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Ethereal.Application.UnitTests.Tests.Commands
{
    [TestFixture]
    public class FetchYoutubeVideoCommandTests : WithInMemoryDatabaseTestBase
    {
        private FetchYoutubeVideoCommand command;
        private ProcessingJob job;
        
        [SetUp]
        public async Task SetUp()
        {
            command = Substitute.Resolve<FetchYoutubeVideoCommand>();
            job = new ProcessingJobBuilder(Fixture, Settings).Build();
            await DbContext.ProcessingJobs.AddAsync(job);
            await DbContext.SaveChangesAsync();
        }

        [Test]
        public async Task CorrectUrl_VideoExists_VideoDownloadedLocally()
        {
            await Task.Delay(1000);
            await command.ExecuteAsync(job);
            
            var expectedDownloadedVideoPath = Path.Combine(job.LocalPath, EtherealApplication.OriginalVideoFileName);
            Assert.That(File.Exists(expectedDownloadedVideoPath));
        }
        
        [Test]
        public async Task CorrectUrl_VideoExists_StatusChanged()
        {
            await Task.Delay(1000);
            await command.ExecuteAsync(job);

            var updatedJob = await DbContext.ProcessingJobs.FirstOrDefaultAsync(j => j.Id == job.Id);
            updatedJob.Status.Should().Be(ProcessingJobStatus.FetchingVideo);
            updatedJob.TotalStepsCount.Should().Be(1);
            updatedJob.CurrentStepIndex.Should().Be(1);
            updatedJob.CurrentStepDescription.Should().Be("Video fetched");
        }
        
        [Test]
        public void CorrectUrl_VideoDoesNotExists_ExceptionExpected()
        {
            job.Video.Id = "ZZZZZZZZZZZZZZZZZZZZZZ";
            job.Video.Url = $"https://youtu.be/{job.Video.Id}";

            var ex = Assert.ThrowsAsync<ArgumentException>(() => command.ExecuteAsync(job));
            Assert.That(ex.Message.Contains("Invalid YouTube video ID or URL"));
        }
        
        [Test]
        public void IncorrectUrl_ExceptionExpected()
        {
            job.Video.Id = "2093418932";
            job.Video.Url = $"https://vk.com/videos/{job.Video.Id}";

            var ex = Assert.ThrowsAsync<ArgumentException>(() => command.ExecuteAsync(job));
            Assert.That(ex.Message.Contains("Invalid YouTube video ID or URL"));
        }
    }
}