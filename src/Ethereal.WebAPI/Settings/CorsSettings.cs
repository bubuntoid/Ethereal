using Microsoft.Extensions.Configuration;

namespace Ethereal.WebAPI.Settings;

public class CorsSettings : SettingsBase
{
    public CorsSettings(IConfiguration config) : base(config, "Cors")
    {
    }

    public string[] Origins => GetArray<string[]>("Origins");
}