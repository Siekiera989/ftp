using Autofac;
using Ftp.Core.DependencyInjection;

namespace Ftp.Core.Extensions;
public static class ContainerBuilderExtensions
{
    public static void RegisterCoreModule(this ContainerBuilder containerBuilder)
    {
        containerBuilder.RegisterModule<LoggerModule>();
        containerBuilder.RegisterModule<SettingsModule>();
        containerBuilder.RegisterModule<ServicesModule>();
    }
}
