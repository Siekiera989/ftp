using Autofac;
using Ftp.Core.Interfaces;
using Ftp.Core.Models;
using Ftp.Host;

namespace Ftp.Tools.App.Host;

public class FtpServerHost : BaseServerHost
{
    public override string Name => nameof(FtpServerHost);

    private static IContainer Container { get; set; }
    private FtpHost ServerHost { get; }

    public FtpServerHost()
    {
        ServerHost = new FtpHost();

        var builder = ServerHost.InitializeContainer();

        builder.RegisterInstance(new ServiceInstanceContext
        {
            ProjectName = "FTP Server"
        }).As<IServiceInstanceContext>().SingleInstance();

        Container = builder.Build();
    }

    public override async Task Run(CancellationToken token)
    {
        await ServerHost.Run(Container, token).ConfigureAwait(false);
    }
}
    