using System.Net;
using Ftp.Command.Abstract;
using Ftp.Core.Connection;
using Ftp.Core.Identity;

namespace Ftp.Command;

public class NoOpCommand : FtpCommandBase
{
    public override string CommandName => "NOOP";

    public override void Execute(FtpConnectionBase user, string arguments)
    {
        user.SendResponse(FtpStatusCode.CommandOK, "Service OK.");
    }
}
