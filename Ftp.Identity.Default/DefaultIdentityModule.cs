using Autofac;
using Ftp.Core.Identity;

namespace Ftp.Identity.Default;

public class DefaultIdentityModule : Module
{
    protected override void Load(ContainerBuilder builder) 
    {
        builder.RegisterType<DefaultAuthenticator>().Keyed(MembershipProviderType.Anonymous, typeof(IFtpAuthenticator)).SingleInstance();
    }
}
