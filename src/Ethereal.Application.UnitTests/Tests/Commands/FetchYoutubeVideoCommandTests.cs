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
    [Ignore("CI/CD")]
    [TestFixture]
    public class FetchYoutubeVideoCommandTests : WithInMemoryDatabaseTestBase
    {
        private FetchYoutubeVideoCommand command;
        private ProcessingJob job;
        
        [SetUp]
        public async Task SetUp()
        {
            await Task.Delay(1000);
            command = Substitute.Resolve<FetchYoutubeVideoCommand>();
            job = new ProcessingJobBuilder(Fixture, Settings).Build();
            await DbContext.ProcessingJobs.AddAsync(job);
            await DbContext.SaveChangesAsync();
        }

        [Test]
        public async Task CorrectUrl_VideoExists_VideoDownloadedLocally()
        {
            await command.ExecuteAsync(job.Id);
            
            var expectedDownloadedVideoPath = Path.Combine(job.LocalPath, Settings.OriginalVideoFileName);
            Assert.That(File.Exists(expectedDownloadedVideoPath));
        }
        
        [Test]
        public async Task CorrectUrl_VideoExists_StatusChanged()
        {
            await command.ExecuteAsync(job.Id);

            var updatedJob = await DbContext.ProcessingJobs.FirstOrDefaultAsync(j => j.Id == job.Id);
            updatedJob.Status.Should().Be(ProcessingJobStatus.Processing);
        }
        
        [Test]
        public void CorrectUrl_VideoDoesNotExists_ExceptionExpected()
        {
            job.Video.Id = "ZZZZZZZZZZZZZZZZZZZZZZ";
            job.Video.Url = $"https://youtu.be/{job.Video.Id}";
            DbContext.SaveChanges();
            
            var ex = Assert.ThrowsAsync<ArgumentException>(() => command.ExecuteAsync(job.Id));
            Assert.That(ex.Message.Contains("Invalid YouTube video ID or URL"));
        }
        
        [Test]
        public void IncorrectUrl_ExceptionExpected()
        {
            job.Video.Id = "2093418932";
            job.Video.Url = $"https://vk.com/videos/{job.Video.Id}";
            DbContext.SaveChanges();
            
            var ex = Assert.ThrowsAsync<ArgumentException>(() => command.ExecuteAsync(job.Id));
            Assert.That(ex.Message.Contains("Invalid YouTube video ID or URL"));
        }
    }
}