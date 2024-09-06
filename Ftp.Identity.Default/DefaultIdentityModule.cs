using Autofac;

namespace Ftp.Identity.Default;

public class DefaultIdentityModule : Module
{
    protected override void Load(ContainerBuilder builder) 
    {
        builder.RegisterType<DefaultAuthenticator>().AsImplementedInterfaces().SingleInstance();
    }
}
