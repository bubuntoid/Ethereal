using System;

namespace Ethereal.Application
{
    public interface IEtherealSettings
    {
        string TempPath { get; }
        
        string ExecutablesPath { get; }

        TimeSpan VideoDurationLimit { get; }
        
        TimeSpan DownloadingTimeout { get; }
    }
}