using Microsoft.Extensions.Configuration;

namespace Ftp.Tools.App.Extensions;

public static class ConfigurationExtensions
{
    public static TSettingsSection GetSettings<TSettingsSection>(this IConfiguration configuration)
        where TSettingsSection : new()
    {
        var section = typeof(TSettingsSection).Name;
        var configurationValue = new TSettingsSection();
        configuration.GetSection(section).Bind(configurationValue);

        return configurationValue;
    }
}
