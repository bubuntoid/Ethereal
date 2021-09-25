using System;
using System.IO;
using Ethereal.Application;
using Microsoft.Extensions.Configuration;

namespace Ethereal.WebAPI.Settings
{
    public class SystemSettings : SettingsBase, IEtherealSettings
    {
        public string TempPath => GetValue<string>("TempPath")
            .Replace("{current}", Directory.GetCurrentDirectory());
        
        public string ExecutablesPath => GetValue<string>("ExecutablesPath")
            .Replace("{current}", Directory.GetCurrentDirectory());

        public TimeSpan VideoDurationLimit => GetValue<TimeSpan>("VideoDurationLimit");

        public TimeSpan DownloadingTimeout => GetValue<TimeSpan>("DownloadingTimeout");
        
        public SystemSettings(IConfiguration config) 
            : base(config, "System")
        {
        }
    }
}