using System.Net;
using Ftp.Command.Abstract;
using Ftp.Core.Connection;
using Ftp.Core.Exceptions;
using Ftp.Core.Identity;

namespace Ftp.Command;

public class PassCommand() : FtpCommandBase()
{
    public override string CommandName => "PASS";

    public override void Execute(FtpConnectionBase connection, string arguments)
    {
        IFtpIdentity identity = connection.Authenticator.AuthenticateUser((string)connection.LastCommandData, arguments);
        if (identity is null)
        {
            throw new FtpException(FtpStatusCode.NotLoggedIn, $"[{CommandName}] Not logged in.");
        }
        else
        {
            connection.Identity = identity; 
            connection.SendResponse(FtpStatusCode.LoggedInProceed, "User logged in.", CommandName);
        }
    }
}
