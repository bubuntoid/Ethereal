using System.IO;
using Microsoft.Extensions.Configuration;

namespace YOVPS.WebAPI.Settings
{
    public class FfmpegSettings : SettingsBase
    {
        public string TempPath => GetValue<string>("TempPath")
            .Replace("{current}", Directory.GetCurrentDirectory());
        
        public string ExecutablesPath => GetValue<string>("ExecutablesPath")
            .Replace("{current}", Directory.GetCurrentDirectory());
        
        public FfmpegSettings(IConfiguration config) 
            : base(config, "FFMPEG")
        {
            
        }
    }
}