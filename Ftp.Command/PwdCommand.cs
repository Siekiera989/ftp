using System.Net;
using Ftp.Command.Abstract;
using Ftp.Core.Connection;
using Serilog;

namespace Ftp.Command;

public class PwdCommand(ILogger logger) : FtpCommandBase(logger)
{
    public override string CommandName => "PWD";

    public override void Execute(FtpConnectionBase user, string arguments)
    {
        user.SendResponse(FtpStatusCode.PathnameCreated, "\"" + user.CurrentDirectory + "\" is current directory.");
        LogInformation(FtpStatusCode.PathnameCreated, "\"" + user.CurrentDirectory + "\" is current directory.");
    }
}
