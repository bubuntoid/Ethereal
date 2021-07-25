using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YOVPS.Core.Extensions;

namespace YOVPS.Core
{
    public static class FfmpegWrapper
    {
        public static string ExecutablesPath { get; set; }

        public static async Task TrimAndSaveToOutputAsync(string path, string output,
            IEnumerable<VideoChapter> chapters, VideoChapter chapter, int index)
        {
            Console.WriteLine($"Processing {chapter.Name}...");
            
            using var p = new Process
            {
                StartInfo =
                {
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    FileName = ExecutablesPath,
                    Arguments =
                        $"-ss {chapter.StartTimespan.TotalSeconds} -i \"{path}\" -t {chapter.Duration.Value.TotalSeconds} -codec copy \"{output}\"",
                }
            };
            p.Start();

            await ComputationExtensions.ComputeElapsedTimeInMillisecondsAsync(
                $"TrimAndSaveToOutput | {chapter.Name} | {index + 1} / {chapters.Count()}",
                p.WaitForExitAsync());
        }
    }
}