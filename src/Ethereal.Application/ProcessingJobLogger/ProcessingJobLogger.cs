﻿using System;
using System.IO;
using System.Threading.Tasks;
using Ethereal.Application.Exceptions;
using Ethereal.Application.Extensions;
using Ethereal.Domain;
using Ethereal.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace Ethereal.Application.ProcessingJobLogger;

public static class ProcessingJobLogger
{
    // ReSharper disable once InconsistentNaming
    public static readonly Logger InternalLogger = LogManager.GetCurrentClassLogger();
    public static IEtherealSettings CurrentSettings { get; set; }
    public static IDatabaseSettings DatabaseSettings { get; set; }

    public static Action<ProcessingJob, string> OnLog { get; set; }

    public static async Task LogAsync(this ProcessingJob _, string message)
    {
        try
        {
            if (DatabaseSettings == null)
                throw new InternalErrorException("DatabaseSettings not initialized");

            await using var dbContext = new EtherealDbContext(DatabaseSettings);
            var job = await dbContext.ProcessingJobs.FirstAsync(j => j.Id == _.Id);
            job.LastLogMessage = message;

            InternalLogger.Info(message);
            OnLog?.Invoke(job, message);

            if (CurrentSettings == null)
                throw new InternalErrorException("CurrentSettings not initialized");

            var directory = Path.Combine(CurrentSettings.TempPath, CurrentSettings.LogsDirectoryName);
            if (File.Exists(directory) == false)
                Directory.CreateDirectory(directory);

            await using var sw = File.AppendText(job.GetLogFilePath(CurrentSettings));
            await sw.WriteLineAsync($"{DateTime.UtcNow} | {job.Id} | {message}\n");
            sw.Close();

            await dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            InternalLogger.Error(ex);
        }
    }
}