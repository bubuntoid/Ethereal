using System;
using System.Threading.Tasks;
using AutoFixture;
using Ethereal.Application.Queries;
using Ethereal.Application.UnitTests.Builders;
using Ethereal.Domain.Entities;
using FluentAssertions;
using NUnit.Framework;

namespace Ethereal.Application.UnitTests.Tests.Queries
{
    [TestFixture]
    public class GetProcessingJobQueryTests : WithInMemoryDatabaseTestBase
    {
        private GetProcessingJobQuery query;
        
        [SetUp]
        public void SetUp()
        {
            query = Substitute.Resolve<GetProcessingJobQuery>();
        }

        [Test]
        public async Task JobExists_JobReturned()
        {
            var existingJob = new ProcessingJobBuilder(Fixture, Settings).Build();
            await DbContext.ProcessingJobs.AddAsync(existingJob);
            await DbContext.SaveChangesAsync();
            
            var job = await query.ExecuteAsync(existingJob.Id);
            
            job.Should().BeEquivalentTo(existingJob);
        }
        
        [Test]
        public void JobDoesNotExists_ExceptionExpected()
        {
            var ex = Assert.ThrowsAsync<Exception>(() => query.ExecuteAsync(Guid.NewGuid()));
        }
    }
}