using Autofac;
using Ftp.Core.Factory;
using Ftp.Core.Services;

namespace Ftp.Core.DependencyInjection;

public class ServicesModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<PasvConnectionFactory>().AsImplementedInterfaces();
        builder.RegisterType<AddressResolver>().AsImplementedInterfaces();
    }
}
