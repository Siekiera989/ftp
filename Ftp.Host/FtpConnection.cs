using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Ftp.Core.FileSystem;
using Ftp.Core.Enums;
using Ftp.Core.Connection;
using Ftp.Core.Exceptions;
using Ftp.Core.Identity;
using Serilog;

namespace Ftp.Host;

public class FtpConnection(FtpHost host, TcpClient client, ILogger logger) : FtpConnectionBase, IDisposable
{
    /// <summary>
    /// The client's user identity, or null if the client is not logged in.
    /// </summary>
    public override IFtpIdentity Identity { get; set; }

    /// <summary>
    /// The filesystem associated with the current <see cref="Identity"/>.  This may throw an exception if the user is not logged in.
    /// </summary>
    public override IFtpFilesystem Filesystem => Identity.Filesystem;

    /// <summary>
    /// The current data connector.
    /// </summary>
    public override IDataConnector DataClient { get; set; }

    /// <summary>
    /// The encoding type to use when transferring information across the data connection.
    /// </summary>
    public override TransferMode TransferType { get; set; } = TransferMode.ASCII;

    /// <summary>
    /// The FTP server that is hosting this client.
    /// </summary>
    public FtpHost Host { get; } = host;

    public override IFtpAuthenticator Authenticator { get; protected set; } = host.Authenticator;

    /// <summary>
    /// Whether the client has been authenticated.
    /// </summary>
    public bool IsLoggedIn => Identity != null;

    /// <summary>
    /// The user's current FTP directory.
    /// </summary>
    public override string CurrentDirectory { get; set; } = "/";

    public override TcpClient ControlClient { get; protected set; } = client;

    private CancellationTokenSource ListeningCancellationSource;
    private NetworkStream ControlClientStream => ControlClient.GetStream();
    private StreamWriter ControlWriter;
    private StreamReader ControlReader;
    private bool _disposedValue;
    private readonly ILogger _logger = logger;

    public override void SendResponse(int code, string data, string commandType)
    {
        _logger.Information("[{FtpCode}][{commandType}] {data}", code, commandType, data);
        ControlWriter.WriteLine(code + " " + data);
        ControlWriter.Flush();
    }

    public override void SendResponse(FtpStatusCode code, string data, string commandType)
    {
        this.SendResponse((int)code, data, commandType);
    }

    public override async Task Start()
    {
        ListeningCancellationSource = new CancellationTokenSource();
        await Listen(ListeningCancellationSource.Token);
    }

    public override void Stop()
    {
        ListeningCancellationSource.Cancel();
        Host.TryRemoveConnection(this);
        Dispose();
    }

    private async Task<string> GetNextLine(CancellationToken token)
    {
        var nextLine = ControlReader.ReadLineAsync(token);
        while (true)
        {
            token.ThrowIfCancellationRequested();

            if (nextLine.IsCompleted)
            {
                return nextLine.Result;
            }

            await Task.Delay(1, token);
        }
    }

    private async Task Listen(CancellationToken token)
    {
        using (ControlWriter = new StreamWriter(ControlClientStream, Encoding.ASCII))
        using (ControlReader = new StreamReader(ControlClientStream, Encoding.ASCII))
        {
            SendResponse(220, "Service ready for new user.", "");
            string line;
            while (!string.IsNullOrEmpty(line = await GetNextLine(token)))
            {
                try
                {
                    Match match = Regex.Match(line, @"^([a-zA-Z]*)(?: )?(.*)$");
                    if (match.Success)
                    {
                        var command = Host.GetCommand(match.Groups[1].Value);
                        if (command is null)
                        {
                            throw new FtpException(502, "Command not implemented.");
                        }
                        else
                        {
                            string arguments = match.Groups.Count > 2 && !string.IsNullOrWhiteSpace(match.Groups[2].Value) ? match.Groups[2].Value : "";
                            command.Execute(this, arguments);
                        }
                    }
                    else
                    {
                        throw new FtpException(FtpStatusCode.ArgumentSyntaxError, "Syntax error in parameters or arguments.");
                    }
                }
                catch (FtpException e)
                {
                    SendResponse(e.StatusCode, e.Message, "");
                }
                catch (Exception)
                {
                    SendResponse(FtpStatusCode.ActionAbortedLocalProcessingError, "Internal server error.", "");
                }
            }
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            _disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~FtpConnection()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}