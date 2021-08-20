using System;

namespace Ethereal.Application
{
    public static class EtherealApplication
    {
        public const string LogsDirectoryName = "Logs";
        
        public const string ThumbnailsDirectoryName = "__thumbnails__";
        
        public const string OriginalVideoFileName = "__original__.mp4";

        public static TimeSpan DefaultFileLifetime = TimeSpan.FromHours(1);
    }
}