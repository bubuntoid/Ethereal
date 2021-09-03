using Ethereal.Domain;
using Microsoft.Extensions.Configuration;

namespace Ethereal.WebAPI.Settings
{
    public class DatabaseSettings : SettingsBase, IDatabaseSettings
    {
        public string ConnectionString => GetValue<string>("ConnectionString");

        public DatabaseSettings(IConfiguration config) 
            : base(config, "Database")
        {
        }
    }
}