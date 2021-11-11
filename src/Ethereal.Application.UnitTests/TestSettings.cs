using System;
using System.IO;

namespace Ethereal.Application.UnitTests
{
    public class TestSettings : IEtherealSettings
    {
        public string TempPath => GetTempDirectory();
        public string FfmpegExecutablesPath { get; set; }
        public string YtdlpExecutablesPath { get; set; }
        public TimeSpan VideoDurationLimit { get; set; } = TimeSpan.FromHours(5);
        public TimeSpan DownloadingTimeout { get; set; } = TimeSpan.FromMinutes(1);
        public string LogsDirectoryName { get; set; } = "Logs";
        public string ThumbnailsDirectoryName { get; set; } = "__thumbnails__";
        public string OriginalVideoFileName { get; set; } = "__original__.mp4";
        public TimeSpan DefaultFileLifetime { get; set; } = TimeSpan.FromHours(1);

        private static string GetTempDirectory()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "temp");
            Directory.CreateDirectory(path);
            return path;
        }
    }
}