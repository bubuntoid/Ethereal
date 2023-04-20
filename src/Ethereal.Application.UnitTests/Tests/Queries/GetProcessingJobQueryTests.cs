using System;
using System.Threading.Tasks;
using Ethereal.Application.Exceptions;
using Ethereal.Application.Queries;
using Ethereal.Application.UnitTests.Builders;
using FluentAssertions;
using NUnit.Framework;

namespace Ethereal.Application.UnitTests.Tests.Queries;

[Ignore("CI/CD")]
[TestFixture]
public class GetProcessingJobQueryTests : WithInMemoryDatabaseTestBase
{
    [SetUp]
    public void SetUp()
    {
        query = Substitute.Resolve<GetProcessingJobQuery>();
    }

    private GetProcessingJobQuery query;

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
        Assert.ThrowsAsync<NotFoundException>(() => query.ExecuteAsync(Guid.NewGuid()));
    }
}