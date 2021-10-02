using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Ethereal.Application.Exceptions;
using Ethereal.Application.Extensions;
using Ethereal.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.LayoutRenderers;

namespace Ethereal.Application.ProcessingJobLogger
{
    public static class ProcessingJobLogger
    {
        public static IEtherealSettings CurrentSettings { get; set; }
        
        public static Action<ProcessingJob, string> OnLog { get; set; }
        
        // ReSharper disable once InconsistentNaming
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public static async Task LogAsync(this ProcessingJob job, DbContext dbContext, string message)
        {
            job.LastLogMessage = message;
            logger.Info(message);
            OnLog?.Invoke(job, message);

            if (CurrentSettings == null)
                throw new InternalErrorException("CurrentSettings not initialized");

            var directory = Path.Combine(CurrentSettings.TempPath, EtherealApplication.LogsDirectoryName);
            if (File.Exists(directory) == false)
                Directory.CreateDirectory(directory);

            await using var sw = File.AppendText(job.GetLogFilePath(CurrentSettings));
            await sw.WriteLineAsync($"{DateTime.UtcNow} | {job.Id} | {message}\n");
            sw.Close();

            await dbContext.SaveChangesAsync();
        }
    }
}