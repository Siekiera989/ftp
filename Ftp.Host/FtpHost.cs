using System.Net.Sockets;
using System.Net;
using Serilog;
using Autofac;
using Ftp.Core.Identity;
using Ftp.Core.Interfaces.Settings;
using Ftp.Core.Extensions;
using Ftp.FileSystem.AzureStorageAccount.DependencyInjection;
using Ftp.Command.Abstract;
using Ftp.Identity.Default;
using Ftp.Command.DependencyInjection;
using Microsoft.ApplicationInsights;
using Ftp.Identity.KeyVault.DependencyInjection;

namespace Ftp.Host;

/// <summary>
/// Creates a new <see cref="FtpHost"/> instance with the specified authenticator.
/// </summary>
/// <param name="authenticator">An FTP authenticator which manages user identities.</param>
public class FtpHost : BaseServerHost
{
    private bool _disposedValue;

    public IFtpAuthenticator Authenticator { get; private set; }

    private Dictionary<string, FtpCommandBase> _indexedCommandSet;
        
    private SynchronizedCollection<FtpConnection> _connections = [];
    private TcpListener _controlServer;
    private static readonly object Locker = new();

    //protected override TelemetryClient Client { get; set; }
    protected override ILogger Logger { get; set; }
    public TelemetryClient Client { get; private set; }

    public override ContainerBuilder InitializeContainer()
    {
        var builder = new ContainerBuilder();

        builder.RegisterCoreModule();
        builder.RegisterModule<StorageAccountModule>();
        builder.RegisterModule<DefaultIdentityModule>();
        builder.RegisterModule<KeyVaultModule>();
        builder.RegisterModule<CommandModule>();

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
            InitializeCommandSet(container);
            _controlServer.Start();
            Logger.Information("[{scope}] Server stared on port: [{port}]", nameof(FtpHost), serverSettings.Port);
            await AcceptNewClientsAsync();
        }
        else
        {
            Logger.Error("[{scope}] Cannot start server", nameof(FtpHost));
        }
    }

    private void InitializeCommandSet(IContainer container)
    {
        var commandSet = container.Resolve<IEnumerable<FtpCommandBase>>();
        _indexedCommandSet = new Dictionary<string, FtpCommandBase>(
            commandSet.Select(x => new KeyValuePair<string, FtpCommandBase>(x.CommandName.ToLowerInvariant(), x)));
    }


    private void ConfigureServer(IContainer container)
    {
        var membershipProvider = container.Resolve<IAuthenticationSettings>().MembershipProvider;
        Authenticator = container.ResolveKeyed<IFtpAuthenticator>(membershipProvider);
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
            FtpConnection connection = new(this, await _controlServer.AcceptTcpClientAsync(), Logger);
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
