using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using YOVPS.Core.Extensions;

namespace YOVPS.Core
{
    public static class FfmpegWrapper
    {
        // ReSharper disable once InconsistentNaming
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        public static string ExecutablesPath { get; set; }

        public static async Task SaveTrimmedAsync(
            string path, 
            string output, 
            VideoChapter chapter, 
            int index,
            int count)
        {
            logger.Info($"Processing {chapter.Name}...");
            
            using var p = new Process
            {
                StartInfo =
                {
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    FileName = ExecutablesPath,
                    Arguments =
                        $"-ss {chapter.StartTimespan.TotalSeconds} -i \"{path}\" -vn -acodec copy -t {chapter.Duration.Value.TotalSeconds} \"{output}\"",
                }
            };
            p.Start();

            await ComputationExtensions.ComputeElapsedTimeInMillisecondsAsync(
                $"SaveTrimmedAsync | {chapter.Name} | {index + 1} / {count}",
                p.WaitForExitAsync());
        }

        public static async Task SaveImageAsync(
            string path,
            string output,
            VideoChapter chapter,
            int index,
            int count
            )
        {
            using var p = new Process
            {
                StartInfo =
                {
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    FileName = ExecutablesPath,
                    Arguments =
                        $"-ss {chapter.StartTimespan.TotalSeconds + 1} -i \"{path}\" -vframes 1 -q:v 2 \"{output}\"",
                }
            };
            p.Start();

            await ComputationExtensions.ComputeElapsedTimeInMillisecondsAsync(
                $"SaveImageAsync | {chapter.Name} | {index + 1} / {count}",
                p.WaitForExitAsync());
        }
    }
}