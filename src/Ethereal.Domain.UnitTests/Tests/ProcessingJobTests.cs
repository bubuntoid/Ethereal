using System.Threading.Tasks;
using AutoFixture;
using Ethereal.Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Ethereal.Domain.UnitTests.Tests;

[TestFixture]
public class ProcessingJobTests : WithRealDatabaseTestBase
{
    [Test]
    public async Task CreateAndLoad()
    {
        var job = Fixture.Create<ProcessingJob>();
        job.Video.ProcessingJobId = job.Id;
        await DbContext.ProcessingJobs.AddAsync(job);
        await DbContext.SaveChangesAsync();

        var loaded = await DbContext.ProcessingJobs
            .Include(j => j.Video)
            .FirstOrDefaultAsync(j => j.Id == job.Id);

        loaded.Should().BeEquivalentTo(job);
    }
}