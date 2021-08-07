using Microsoft.Extensions.Configuration;

namespace YOVPS.WebAPI.Settings
{
    public abstract class SettingsBase
    {
        protected readonly IConfiguration config;
        private readonly string section;

        protected SettingsBase(IConfiguration config, string section)
        {
            this.config = config;
            this.section = section;
        }

        protected T GetValue<T>(string key)
        {
            return config.GetSection(section).GetValue<T>(key);
        }
    }
}