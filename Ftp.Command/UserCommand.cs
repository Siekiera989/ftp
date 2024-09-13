using System.Net;
using Ftp.Command.Abstract;
using Ftp.Core.Connection;

namespace Ftp.Command;

public class UserCommand() : FtpCommandBase()
{
    public override string CommandName => "USER";

    public override void Execute(FtpConnectionBase user, string arguments)
    {
        user.LastCommandData = arguments;
        user.SendResponse(FtpStatusCode.SendPasswordCommand, "Username ok, need password", CommandName);
    }
}
