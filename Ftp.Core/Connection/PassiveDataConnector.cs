using System.Net;
using System.Net.Sockets;
using Ftp.Core.Interfaces.Settings;

namespace Ftp.Core.Connection;

/// <summary>
/// Represents a "passive" data connector, which creates data connections by listening for clients on a specific server-side port.
/// </summary>
public class PassiveDataConnector : IDataConnector
{
    /// <summary>
    /// The current <see cref="TcpListener"/> used to listen for incoming passive data connections.
    /// </summary>
    public TcpListener DataListener;

    public PassiveDataConnector(IPAddress address, int port)
    { 
        DataListener = new TcpListener(address, port);
        DataListener.Start();
    }

    /// <summary>
    /// Creates a new data connection.
    /// </summary>
    /// <returns>A <see cref="TcpClient"/> that can be used to transfer data.  The client should be disposed when no longer in use.</returns>
    public async Task<TcpClient> CreateDataConnectionAsync()
    {
        return await DataListener.AcceptTcpClientAsync();
    }

    /// <summary>
    /// Disposes the connector, closing any active data connections in the process.
    /// </summary>
    public void Dispose()
    {
        DataListener.Stop();
    }
}
