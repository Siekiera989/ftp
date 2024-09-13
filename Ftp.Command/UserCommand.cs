using System.Net;
using Ftp.Command.Abstract;
using Ftp.Core.Connection;
using Serilog;

namespace Ftp.Command;

public class UserCommand(ILogger logger) : FtpCommandBase(logger)
{
    public override string CommandName => "USER";

    public override void Execute(FtpConnectionBase user, string arguments)
    {
        user.LastCommandData = arguments;
        user.SendResponse(FtpStatusCode.SendPasswordCommand, "Username ok, need password");
        LogInformation(FtpStatusCode.SendPasswordCommand, "Username ok, need password");
    }
}
