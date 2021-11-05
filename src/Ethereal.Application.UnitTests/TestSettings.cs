using System;
using System.IO;

namespace Ethereal.Application.UnitTests
{
    public class TestSettings : IEtherealSettings
    {
        public string TempPath => GetTempDirectory();
        public string FfmpegExecutablesPath { get; set; }
        public string YtdlpExecutablesPath { get; set; }
        public TimeSpan VideoDurationLimit { get; } = TimeSpan.FromHours(5);
        
        public TimeSpan DownloadingTimeout { get; } = TimeSpan.FromMinutes(1);

        private static string GetTempDirectory()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "temp");
            Directory.CreateDirectory(path);
            return path;
        }
    }
}