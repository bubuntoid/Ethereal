using System;
using System.Diagnostics;
using System.IO;

namespace YOVPS.Core
{
    public static class FfmpegWrapper
    {
        public static string ExecutablesPath { get; set; }

        public static void TrimAndSaveToOutput(string path, string output, TimeSpan from, TimeSpan to)
        {
            using var p = new Process
            {
                StartInfo =
                {
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    FileName = Path.Combine(ExecutablesPath, IsLinux ? "ffmpeg" : "ffmpeg.exe"),
                    Arguments = $"-i \"{path}\" -ss {from.TotalSeconds} -t {to.TotalSeconds} \"{output}\"",
                }
            };
            p.Start();
            p.WaitForExit();
        }

        private static bool IsLinux
        {
            get
            {
                var p = (int) Environment.OSVersion.Platform;
                return p is 4 or 6 or 128;
            }
        }
    }
}