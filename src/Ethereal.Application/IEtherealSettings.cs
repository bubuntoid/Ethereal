using System;

namespace Ethereal.Application
{
    public interface IEtherealSettings
    {
        string TempPath { get; }
        
        string FfmpegExecutablesPath { get; }

        string YtdlpExecutablesPath { get; }

        TimeSpan VideoDurationLimit { get; }
        
        TimeSpan DownloadingTimeout { get; }
    }
}