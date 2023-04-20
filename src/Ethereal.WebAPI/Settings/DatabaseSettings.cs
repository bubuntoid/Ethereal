using Ethereal.Domain;
using Microsoft.Extensions.Configuration;

namespace Ethereal.WebAPI.Settings;

public class DatabaseSettings : SettingsBase, IDatabaseSettings
{
    public DatabaseSettings(IConfiguration config)
        : base(config, "Database")
    {
    }

    public string ConnectionString => GetValue<string>("ConnectionString");
}