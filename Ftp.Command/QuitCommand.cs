using System.Net;
using Ftp.Command.Abstract;
using Ftp.Core.Connection;
using Serilog;

namespace Ftp.Command;

public sealed class QuitCommand(ILogger logger) : FtpCommandBase(logger)
{
    public override string CommandName => "QUIT";

    public override void Execute(FtpConnectionBase user, string arguments)
    {
        user.SendResponse(FtpStatusCode.ClosingControl, "Service closing control connection.");
        LogInformation(FtpStatusCode.ClosingControl, "Service closing control connection.");
    }
}
