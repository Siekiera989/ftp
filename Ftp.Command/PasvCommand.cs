using System.Net;
using Ftp.Command.Abstract;
using Ftp.Core.Connection;
using Ftp.Core.Identity;

namespace Ftp.Command;

public class PasvCommand : FtpCommandBase
{
    public override string CommandName => "PASV";

    public override void Execute(FtpConnectionBase user, string arguments)
    {
        user.DataClient?.Dispose();
        PassiveDataConnector connection = new();
        user.DataClient = connection;
        byte[] address = ((IPEndPoint)user.ControlClient.Client.LocalEndPoint).Address.GetAddressBytes();
        byte[] port = BitConverter.GetBytes((short)((IPEndPoint)connection.DataListener.LocalEndpoint).Port);
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(port);
        }
        user.SendResponse(FtpStatusCode.EnteringPassive, $"Entering Passive Mode ({address[0]},{address[1]},{address[2]},{address[3]},{port[0]},{port[1]})");
    }
}