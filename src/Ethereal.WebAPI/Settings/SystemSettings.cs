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
        
        public SystemSettings(IConfiguration config) 
            : base(config, "FFMPEG")
        {
        }
    }
}