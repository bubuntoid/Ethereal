using System.IO;
using AutoFixture;
using Ethereal.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ethereal.Application.UnitTests.Builders
{
    public class ProcessingJobBuilder
    {
        private readonly IFixture fixture;

        public ProcessingJobBuilder(IFixture fixture)
        {
            this.fixture = fixture;
        }
        
        public ProcessingJob Build()
        {
            var job = fixture.Create<ProcessingJob>();
            job.Video.ProcessingJobId = job.Id;
            job.Video.Id = "TmQyfUpyeFk";
            job.Video.Url = $"https://youtu.be/{job.Video.Id}";

            job.LocalPath = Path.Combine(TestConfiguration.GetTempDirectory(), $"{job.Id}");
            Directory.CreateDirectory(job.LocalPath);
            
            return job;
        }
    }
}