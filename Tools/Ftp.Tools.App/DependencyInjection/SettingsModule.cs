using Autofac;
using Ftp.Core.Models.Settings;
using Ftp.Tools.App.Extensions;
using Microsoft.Extensions.Configuration;

namespace Ftp.Tools.App.DependencyInjection;

public class SettingsModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        IConfigurationRoot config = new ConfigurationBuilder()
        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
        .AddEnvironmentVariables()
        .AddJsonFile("appsettings.json", false, true)
        .AddJsonFile($"appsettings.{environmentName}.json", true)
        .Build();

        builder.RegisterInstance(config).As<IConfigurationRoot>();

        var loggerSettings = config.GetSettings<LoggerSettings>();
        var storageAccountSettings = config.GetSettings<StorageAccountSettings>();

        builder.Register(x => x.Resolve<IConfigurationRoot>().GetSettings<ServerSettings>()).AsImplementedInterfaces();
        builder.Register(x => loggerSettings).AsImplementedInterfaces();
        builder.Register(x => storageAccountSettings).AsImplementedInterfaces();
    }
}
