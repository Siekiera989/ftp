using Autofac;
using Ftp.Core.Extensions;
using Ftp.Core.Models.Settings;
using Microsoft.Extensions.Configuration;

namespace Ftp.Core.DependencyInjection;

public class SettingsModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        IConfigurationRoot config = new ConfigurationBuilder()
        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
        .AddJsonFile("appsettings.json", false, true)
        .Build();

        builder.RegisterInstance(config).As<IConfigurationRoot>();

        var serverSettings = config.GetSettings<ServerSettings>();
        var loggerSettings = config.GetSettings<LoggerSettings>();
        var storageAccountSettings = config.GetSettings<StorageAccountSettings>();
        var passiveConnectionSettings = config.GetSettings<PassiveConnectionSettings>();
        var keyVaultSettings = config.GetSettings<KeyVaultSettings>();
        var authenticationSettings = config.GetSettings<AuthenticationSettings>();

        builder.Register(x => serverSettings).AsImplementedInterfaces();
        builder.Register(x => loggerSettings).AsImplementedInterfaces();
        builder.Register(x => storageAccountSettings).AsImplementedInterfaces();
        builder.Register(x => passiveConnectionSettings).AsImplementedInterfaces();
        builder.Register(x => keyVaultSettings).AsImplementedInterfaces();
        builder.Register(x => authenticationSettings).AsImplementedInterfaces();
    }
}
