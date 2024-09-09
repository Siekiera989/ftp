using System.Net;
using Ftp.Command.Abstract;
using Ftp.Core.Connection;
using Ftp.Core.Factory;

namespace Ftp.Command;

public class PasvCommand : FtpCommandBase
{
    private readonly IPasvConnectionFactory _pasvConnectionFactory;

    public PasvCommand(IPasvConnectionFactory pasvConnectionFactory)
    {
        _pasvConnectionFactory = pasvConnectionFactory;
    }

    public override string CommandName => "PASV";

    public override void Execute(FtpConnectionBase user, string arguments)
    {
        user.DataClient?.Dispose();
        var localAddress = (user.ControlClient.Client.LocalEndPoint as IPEndPoint).Address;
        PassiveDataConnector connection = _pasvConnectionFactory.CreatePassiveDataConnector(localAddress);

        user.DataClient = connection;
        byte[] address = localAddress.GetAddressBytes();
        byte[] port = BitConverter.GetBytes((short)((IPEndPoint)connection.DataListener.LocalEndpoint).Port);
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(port);
        }
        user.SendResponse(FtpStatusCode.EnteringPassive, $"Entering Passive Mode ({address[0]},{address[1]},{address[2]},{address[3]},{port[0]},{port[1]})");
    }
}