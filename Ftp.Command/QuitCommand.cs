using System.Net;
using Ftp.Command.Abstract;
using Ftp.Core.Connection;
using Ftp.Core.Identity;

namespace Ftp.Command;

public sealed class QuitCommand : FtpCommandBase
{
    public override string CommandName => "QUIT";

    public override void Execute(FtpConnectionBase user, string arguments)
    {
        user.SendResponse(FtpStatusCode.ClosingControl, "Service closing control connection.");
    }
}
