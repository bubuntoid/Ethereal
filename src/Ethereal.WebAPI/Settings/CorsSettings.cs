using Microsoft.Extensions.Configuration;

namespace Ethereal.WebAPI.Settings
{
    public class CorsSettings : SettingsBase
    {
        public string[] Origins => GetArray<string[]>("Origins");
        
        public CorsSettings(IConfiguration config) : base(config, "Cors")
        {
            
        }
    }
}