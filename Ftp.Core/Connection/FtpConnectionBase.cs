using System.Net;
using System.Net.Sockets;
using Ftp.Core.Enums;
using Ftp.Core.FileSystem;
using Ftp.Core.Identity;

namespace Ftp.Core.Connection;

public abstract class FtpConnectionBase
{


    /// <summary>
    /// An object which contains data put there by the last command used.  Two-part commands, like USER and PASS, use this object to pass data between each other.
    /// </summary>
    public object LastCommandData;
    /// <summary>
    /// The client's user identity, or null if the client is not logged in.
    /// </summary>
    public abstract IFtpIdentity Identity { get; set; }

    /// <summary>
    /// The filesystem associated with the current <see cref="Identity"/>.  This may throw an exception if the user is not logged in.
    /// </summary>
    public abstract IFtpFilesystem Filesystem { get; }

    /// <summary>
    /// The current data connector.
    /// </summary>
    public abstract IDataConnector DataClient { get; set; }

    /// <summary>
    /// The encoding type to use when transferring information across the data connection.
    /// </summary>
    public abstract TransferMode TransferType { get; set; }

    /// <summary>
    /// The object which manages the authentication of users attempting to log into the FTP system.
    /// </summary>
    public abstract IFtpAuthenticator Authenticator { get; protected set; }

    /// <summary>
    /// The user's current FTP directory.
    /// </summary>
    public abstract string CurrentDirectory { get; set; }

    public abstract TcpClient ControlClient { get; protected set; }

    /// <summary>
    /// Sends an FTP response to the client.  FTP responses consist of a status code and a single-line message.
    /// </summary>
    /// <param name="code">The FTP status code to send.</param>
    /// <param name="data">The message to send along with the status code.</param>
    public abstract void SendResponse(FtpStatusCode code, string data);

    /// <summary>
    /// Sends an FTP response to the client.  FTP responses consist of a status code and a single-line message.
    /// </summary>
    /// <param name="code">The FTP status code to send.</param>
    /// <param name="data">The message to send along with the status code.</param>
    public abstract void SendResponse(int code, string data);

    public abstract Task Start();

    /// <summary>
    /// Stops the FTP connection and disconnects the user from the server.
    /// </summary>
    public abstract void Stop();
}
