using System.IO;
using AutoFixture;
using Ethereal.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ethereal.Application.UnitTests.Builders
{
    public class ProcessingJobBuilder
    {
        private readonly IFixture fixture;
        private readonly TestSettings settings;

        public ProcessingJobBuilder(IFixture fixture, TestSettings settings)
        {
            this.fixture = fixture;
            this.settings = settings;
        }
        
        public ProcessingJob Build()
        {
            var job = fixture.Create<ProcessingJob>();
            job.Video.ProcessingJobId = job.Id;
            job.Video.Id = "TmQyfUpyeFk";
            job.Video.Url = $"https://youtu.be/{job.Video.Id}";

            job.LocalPath = Path.Combine(settings.TempPath, $"{job.Id}");
            Directory.CreateDirectory(job.LocalPath);
            
            return job;
        }
    }
}