using Autofac;
using Azure.Security.KeyVault.Secrets;
using Ftp.Core.Identity;
using Ftp.Core.Interfaces.Settings;
using Ftp.Identity.KeyVault.Factories;

namespace Ftp.Identity.KeyVault.DependencyInjection;

public class KeyVaultModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.Register(c =>
        {
            var options = c.Resolve<IKeyVaultSettings>();
            var keyVaultSecretClient = SecretClientFactory.Create(options.KeyVaultUri);
            return keyVaultSecretClient;

        }).As<SecretClient>().SingleInstance();

        builder.RegisterType<KeyVaultAuthenticator>().Keyed(MembershipProviderType.KeyVault, typeof(IFtpAuthenticator)).SingleInstance();
    }
}
