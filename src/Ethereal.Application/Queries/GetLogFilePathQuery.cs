using System;
using System.IO;
using System.Threading.Tasks;
using Ethereal.Application.Exceptions;
using Ethereal.Application.Extensions;
using Ethereal.Domain;
using Microsoft.EntityFrameworkCore;

namespace Ethereal.Application.Queries;

public class GetLogFilePathQuery
{
    private readonly EtherealDbContext dbContext;
    private readonly IEtherealSettings settings;

    public GetLogFilePathQuery(EtherealDbContext dbContext, IEtherealSettings settings)
    {
        this.dbContext = dbContext;
        this.settings = settings;
    }

    public async Task<string> ExecuteAsync(Guid jobId)
    {
        var job = await dbContext.ProcessingJobs
            .Include(j => j.Video)
            .FirstOrDefaultAsync(j => j.Id == jobId);

        if (job == null)
            throw new NotFoundException("Job not found");

        var path = Path.Combine(job.LocalPath, job.GetLogFilePath(settings));

        if (File.Exists(path) == false)
            throw new NotFoundException("File not found");

        return path;
    }
}