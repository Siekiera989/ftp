using System.Net;
using Ftp.Command.Abstract;
using Ftp.Core.Connection;
using Ftp.Core.Exceptions;
using Ftp.Core.Identity;
using Serilog;

namespace Ftp.Command;

public class PassCommand(ILogger logger) : FtpCommandBase(logger)
{
    public override string CommandName => "PASS";

    public override void Execute(FtpConnectionBase connection, string arguments)
    {
        IFtpIdentity identity = connection.Authenticator.AuthenticateUser((string)connection.LastCommandData, arguments);
        if (identity is null)
        {
            LogError(FtpStatusCode.NotLoggedIn, "Not logged in.");
            throw new FtpException(FtpStatusCode.NotLoggedIn, "Not logged in.");
        }
        else
        {
            connection.Identity = identity;
            connection.SendResponse(FtpStatusCode.LoggedInProceed, "User logged in.");
            LogInformation(FtpStatusCode.LoggedInProceed, "User logged in.");
        }
    }
}
