using System.Net;
using Ftp.Command.Abstract;
using Ftp.Core.Connection;
using Serilog;

namespace Ftp.Command;

public class NoOpCommand(ILogger logger) : FtpCommandBase(logger)
{
    public override string CommandName => "NOOP";

    public override void Execute(FtpConnectionBase user, string arguments)
    {
        LogInformation(FtpStatusCode.CommandOK, "Service OK.");
        user.SendResponse(FtpStatusCode.CommandOK, "Service OK.");
    }
}
