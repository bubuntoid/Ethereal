using System.Diagnostics;
using System.Threading.Tasks;
using Ethereal.Application.Extensions;
using NLog;

namespace Ethereal.Application
{
    public class FfmpegWrapper
    {
        private readonly Logger logger = LogManager.GetCurrentClassLogger();
        private string ExecutablesPath { get; }

        public FfmpegWrapper(IEtherealSettings settings)
        {
            ExecutablesPath = settings.FfmpegExecutablesPath;
        }
        
        public async Task SaveTrimmedAsync(
            string path, 
            string output, 
            VideoChapter chapter)
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
                $"SaveTrimmedAsync | {chapter.Name}",
                p.WaitForExitAsync());
        }

        public async Task SaveImageAsync(
            string path,
            string output,
            VideoChapter chapter)
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
                        $"-ss {chapter.StartTimespan.TotalSeconds} -i \"{path}\" -frames:v 1 \"{output}\"",
                }
            };
            p.Start();

            await ComputationExtensions.ComputeElapsedTimeInMillisecondsAsync(
                $"SaveImageAsync | {chapter.Name}",
                p.WaitForExitAsync());
        }
    }
}