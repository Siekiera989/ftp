using System.Net.Sockets;
using System.Net;
using Serilog;
using Autofac;
using Ftp.Core.Identity;
using Ftp.Core.Interfaces.Settings;
using Ftp.Core.DependencyInjection;
using Ftp.Core.Extensions;
using Ftp.FileSystem.AzureStorageAccount.DependencyInjection;
using Ftp.Command.Abstract;
using Ftp.Command;
using Ftp.Identity.Default;

namespace Ftp.Host;

/// <summary>
/// Creates a new <see cref="FtpHost"/> instance with the specified authenticator.
/// </summary>
/// <param name="authenticator">An FTP authenticator which manages user identities.</param>
public class FtpHost : BaseServerHost
{
    public FtpHost(IEnumerable<FtpCommandBase> commandSet)
    {
        _indexedCommandSet = new Dictionary<string, FtpCommandBase>(
            commandSet.Select(x => new KeyValuePair<string, FtpCommandBase>(x.CommandName.ToLowerInvariant(), x)));
    }

    private bool _disposedValue;

    public IFtpAuthenticator Authenticator { get; private set; }

    private readonly Dictionary<string, FtpCommandBase> _indexedCommandSet;
        
    private SynchronizedCollection<FtpConnection> _connections = [];
    private TcpListener _controlServer;
    private static readonly object Locker = new();

    //protected override TelemetryClient Client { get; set; }
    protected override ILogger Logger { get; set; }

    public override ContainerBuilder InitializeContainer()
    {
        var builder = new ContainerBuilder();
        builder.RegisterModule<LoggerModule>();
        builder.RegisterModule<DefaultIdentityModule>();
        builder.RegisterModule<StorageAccountModule>();

        builder.RegisterType<CwdCommand>().As<FtpCommandBase>().SingleInstance();
        builder.RegisterType<DeleCommand>().As<FtpCommandBase>().SingleInstance();
        builder.RegisterType<ListCommand>().As<FtpCommandBase>().SingleInstance();
        builder.RegisterType<MkdCommand>().As<FtpCommandBase>().SingleInstance();
        builder.RegisterType<NoOpCommand>().As<FtpCommandBase>().SingleInstance();
        builder.RegisterType<PassCommand>().As<FtpCommandBase>().SingleInstance();
        builder.RegisterType<PasvCommand>().As<FtpCommandBase>().SingleInstance();
        builder.RegisterType<PortCommand>().As<FtpCommandBase>().SingleInstance();
        builder.RegisterType<PwdCommand>().As<FtpCommandBase>().SingleInstance();
        builder.RegisterType<QuitCommand>().As<FtpCommandBase>().SingleInstance();
        builder.RegisterType<RetrCommand>().As<FtpCommandBase>().SingleInstance();
        builder.RegisterType<RmdaCommand>().As<FtpCommandBase>().SingleInstance();
        builder.RegisterType<RmdCommand>().As<FtpCommandBase>().SingleInstance();
        builder.RegisterType<RnfrCommand>().As<FtpCommandBase>().SingleInstance();
        builder.RegisterType<RntoCommand>().As<FtpCommandBase>().SingleInstance();
        builder.RegisterType<SizeCommand>().As<FtpCommandBase>().SingleInstance();
        builder.RegisterType<StorCommand>().As<FtpCommandBase>().SingleInstance();
        builder.RegisterType<TypeCommand>().As<FtpCommandBase>().SingleInstance();
        builder.RegisterType<UserCommand>().As<FtpCommandBase>().SingleInstance();

        return builder;
    }

    public override async Task Run(IContainer container, CancellationToken token)
    {
        var serverSettings = container.Resolve<IServerSettings>();
        lock (Locker)
        {
            if (_controlServer != null)
            {
                throw new InvalidOperationException("The server is already running.");
            }

            if (IPAddress.TryParse(IpAddressExtensions.GetLocalIpAddress(), out IPAddress ipAddress))
            {
                _controlServer = new TcpListener(ipAddress, serverSettings.Port);
            }
        }

        if (_controlServer != null)
        {
            ConfigureServer(container);
            _controlServer.Start();
            await AcceptNewClientsAsync();
            Logger.Information("[{scope}] Server stared on port: [{port}]", nameof(FtpHost), serverSettings.Port);
        }
        else
        {
            Logger.Error("[{scope}] Cannot start server", nameof(FtpHost));
        }
    }


    private void ConfigureServer(IContainer container)
    {
        Authenticator = container.Resolve<IFtpAuthenticator>();
        Logger = container.Resolve<ILogger>();
        //Client = container.Resolve<TelemetryClient>();
    }

    /// <summary>
    /// Stops the FTP server, severing all client connections.
    /// </summary>
    private void Stop()
    {
        lock (Locker)
        {
            _controlServer.Stop();
            foreach (FtpConnection user in _connections.ToArray())
            {
                try
                {
                    user.Stop();
                }
                catch { }
            }
            _controlServer = null;
        }
    }

    private async Task AcceptNewClientsAsync()
    {
        while (_controlServer != null)
        {
            FtpConnection connection = new(this, await _controlServer.AcceptTcpClientAsync());
            _connections.Add(connection);
            await connection.Start();
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                Stop();
            }

            _disposedValue = true;
        }
    }

    ~FtpHost()
    {
        Dispose(disposing: false);
    }

    public override void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public void TryRemoveConnection(FtpConnection ftpConnection)
    {
        _connections.Remove(ftpConnection);
    }

    public FtpCommandBase GetCommand(string commandName)
    {
        _indexedCommandSet.TryGetValue(commandName.ToLowerInvariant(), out FtpCommandBase toReturn);
        return toReturn;
    }
}
