using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Ethereal.Application.Exceptions;
using Ethereal.Application.Extensions;
using Ethereal.Application.ProcessingJobLogger;
using Ethereal.Domain;
using Ethereal.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using TagLib;

namespace Ethereal.Application.Commands;

public class ConvertVideoCommand
{
    private readonly EtherealDbContext dbContext;
    private readonly FfmpegWrapper ffmpegWrapper;
    private readonly IEtherealSettings settings;

    public ConvertVideoCommand(EtherealDbContext dbContext, FfmpegWrapper ffmpegWrapper, IEtherealSettings settings)
    {
        this.dbContext = dbContext;
        this.ffmpegWrapper = ffmpegWrapper;
        this.settings = settings;
    }

    public async Task ExecuteAsync(Guid jobId)
    {
        var job = await dbContext.ProcessingJobs
            .Include(j => j.Video)
            .FirstOrDefaultAsync(j => j.Id == jobId);

        if (job == null)
            throw new NotFoundException();

        var chapters = job.ParseChapters();
        job.Status = ProcessingJobStatus.Processing;
        await dbContext.SaveChangesAsync();

for (var i = 0; i < chapters.Count; i++)
{
    var chapter = chapters.ElementAt(i);

    await job.LogAsync($"Converting video [{i + 1}/{chapters.Count}] ({chapter.Name})");

    await ffmpegWrapper.SaveTrimmedAsync(job.GetLocalVideoPath(settings), job.GetChapterLocalFilePath(chapter), chapter);

	// Add metadata to the split file
	var filePath = job.GetChapterLocalFilePath(chapter);
	var file = TagLib.File.Create(filePath);
	file.Tag.Title = chapter.Name;
	file.Tag.Album = job.Video.Title;  // Or whatever you want the album to be
	file.Tag.Comment = "Split with Ethereal";

	// Add the thumbnail as album art
	var thumbnailPath = System.IO.Path.Combine(settings.TempPath, job.Id.ToString(), settings.ThumbnailsDirectoryName, i + ".png");  // Use the chapter number as the file name

	if (System.IO.File.Exists(thumbnailPath))
	{
	    var picture = new TagLib.Picture(thumbnailPath);
	    file.Tag.Pictures = new TagLib.IPicture[] { picture };
	}
	else
	{
	    // If the specific thumbnail doesn't exist, try to use a different one
	    var thumbnailDirectory = System.IO.Path.Combine(settings.TempPath, job.Id.ToString(), settings.ThumbnailsDirectoryName);
	    var thumbnails = System.IO.Directory.GetFiles(thumbnailDirectory, "*.png");
	    if (thumbnails.Length > 0)
	    {
		var picture = new TagLib.Picture(thumbnails[0]);  // Use the first available thumbnail
		file.Tag.Pictures = new TagLib.IPicture[] { picture };
	    }
	    // If no thumbnails are available, don't set the album art
	}

	file.Save();

	}

        await dbContext.SaveChangesAsync();
        await job.LogAsync("Converting succeeded");
    }
}
