#nullable enable
using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Ethereal.Application.Extensions;
using Ethereal.Application.ProcessingJobLogger;
using Ethereal.Domain.Entities;
using YoutubeExplode;
#pragma warning disable 8618

namespace Ethereal.Application.YouTube
{
    // todo: refactor me
    public class YtdlpProvider : IYoutubeProvider
    {
        private readonly IEtherealSettings settings;

        private readonly string[] outputSeparators = {
            "[download]",
            "of",
            "at",
            "ETA",
            " "
        };
        
        private Process process { get; set; }

        private double DownloadButtonProgressPercentageValue { get; set; }
        private string DownloadButtonProgressPercentageString { get; set; } = "_Download";

        public YtdlpProvider(IEtherealSettings settings)
        {
            this.settings = settings;
        }
        
        public async Task<YoutubeVideo> GetVideoAsync(string url)
        {
            // todo: dry
            var video = await new YoutubeClient().Videos.GetAsync(url);
            return new YoutubeVideo
            {
                Description = video.Description,
                Id = video.Id,
                Duration = video.Duration,
                Title = video.Title,
            };
        }

        public async Task DownloadAsync(ProcessingJob job, CancellationTokenSource cts)
        {
            try
            {
                PrepareDlProcess(job);
                StartDownload(job);
            }
            catch (Exception e)
            {
                await job.LogAsync(e.Message);
                throw;
            }
        }

        private void PrepareDlProcess(ProcessingJob job)
        {
            process = new Process
            {
                StartInfo =
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    StandardErrorEncoding = Encoding.UTF8,
                    StandardOutputEncoding = Encoding.UTF8
                },
                EnableRaisingEvents = true
            };
            process.ErrorDataReceived += (sender, args) => DlOutputHandler(job, sender, args);
            process.OutputDataReceived += (sender, args) => DlOutputHandler(job, sender, args);
            process.Exited += DlProcess_Exited;
        }

        private void DlOutputHandler(ProcessingJob job, object? sendingProcess, DataReceivedEventArgs outLine)
        {
            if (!string.IsNullOrEmpty(outLine.Data))
            {
                ParseDlOutput(outLine.Data, job);
            }
            
            // todo: handle error
        }

        private void DlProcess_Exited(object? sender, EventArgs e)
        {
            process.CancelErrorRead();
            process.CancelOutputRead();
            
            // todo: fail job here?
            
            // FreezeButton = false;
            // DownloadButtonProgressIndeterminate = false;
            // FormatsButtonProgressIndeterminate = false;
            DownloadButtonProgressPercentageValue = 0.0;
            DownloadButtonProgressPercentageString = "_Download";
        }

        private void StartDownload(ProcessingJob job)
        {
            process.StartInfo.FileName = settings.YtdlpExecutablesPath;
            process.StartInfo.ArgumentList.Clear();

            try
            {
                // Use '-f' if no specified format. With specified format, use '--merge-output-format'.
                process.StartInfo.ArgumentList.Add("-f");
                process.StartInfo.ArgumentList.Add($"mp4");

                process.StartInfo.ArgumentList.Add("-o");
                process.StartInfo.ArgumentList.Add(job.GetLocalVideoPath(settings));

                process.StartInfo.ArgumentList.Add(job.Video.Url);

                process.Start();
                process.BeginErrorReadLine();
                process.BeginOutputReadLine();

                // FreezeButton = true;
                // DownloadButtonProgressIndeterminate = true;

                process.WaitForExit();
            }
            catch
            {
                // outputString.Append(ex.Message);
                // outputString.Append(Environment.NewLine);
                // Output = outputString.ToString();
                
                // todo: log error to job
            }
            finally
            {
            }
        }

        private void ParseDlOutput(string output, ProcessingJob job)
        {
            var parsedStringArray = output.Split(outputSeparators, StringSplitOptions.RemoveEmptyEntries);
            if (parsedStringArray.Length == 4) // valid [download] line
            {
                var percentageString = parsedStringArray[0];
                if (percentageString.EndsWith('%')) // actual percentage
                {
                    // show percentage on button
                    DownloadButtonProgressPercentageString = percentageString;

                    // get percentage value for progress bar
                    var percentageNumberString = percentageString.TrimEnd('%');
                    if (double.TryParse(percentageNumberString, out var percentageNumber))
                    {
                        DownloadButtonProgressPercentageValue = percentageNumber;
                        var message = $"Fetching video... [{percentageNumber}%]";
                        ProcessingJobLogger.ProcessingJobLogger.InternalLogger.Info(message);
                        ProcessingJobLogger.ProcessingJobLogger.OnLog(job, message);
                    }
                }
            }
        }
    }
}