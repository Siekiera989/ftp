using System.Net;
using Ftp.Command.Abstract;
using Ftp.Core.Connection;

namespace Ftp.Command;

public class PwdCommand() : FtpCommandBase()
{
    public override string CommandName => "PWD";

    public override void Execute(FtpConnectionBase user, string arguments)
    {
        user.SendResponse(FtpStatusCode.PathnameCreated, $"\"" + user.CurrentDirectory + "\" is current directory.", CommandName);
    }
}
